/**
 * ETL SQL Server (TONIO_GARAGE) → Supabase Postgres.
 *
 * Étapes :
 *   1. Connexion source (mssql) + cible (supabase service_role)
 *   2. Lecture des compteurs sources, affichage
 *   3. Si pas --dry-run :
 *      a. RPC etl_truncate_all_data        (vide les tables cibles)
 *      b. Insert table par table (ordre des FK)
 *      c. RPC etl_reset_all_sequences      (réaligne les sequences)
 *      d. Lecture des compteurs cibles, comparaison avec les sources
 *
 * Toutes les insertions se font avec id explicite pour préserver les
 * références historiques (NumDevis, NumFacture, etc.).
 *
 * Usage :
 *   npm run migrate         # exécution réelle
 *   npm run migrate:dry     # affichage des compteurs uniquement
 */

import "dotenv/config";
import sql from "mssql";
import { createClient, SupabaseClient } from "@supabase/supabase-js";

const DRY_RUN = process.argv.includes("--dry-run");

// ----------------------------------------------------------------------------
// Connexions
// ----------------------------------------------------------------------------

function requireEnv(name: string): string {
  const v = process.env[name];
  if (!v) throw new Error(`Variable d'environnement manquante : ${name}`);
  return v;
}

async function connectMssql(): Promise<sql.ConnectionPool> {
  return await sql.connect({
    server: requireEnv("MSSQL_HOST"),
    port: Number(process.env.MSSQL_PORT ?? 1433),
    user: requireEnv("MSSQL_USER"),
    password: requireEnv("MSSQL_PASS"),
    database: requireEnv("MSSQL_DB"),
    options: { encrypt: true, trustServerCertificate: true },
    pool: { max: 4, min: 0 },
    requestTimeout: 60_000,
  });
}

function connectSupabase(): SupabaseClient {
  return createClient(
    requireEnv("SUPABASE_URL"),
    requireEnv("SUPABASE_SERVICE_ROLE_KEY"),
    { auth: { persistSession: false }, db: { schema: "public" } },
  );
}

// ----------------------------------------------------------------------------
// Helpers
// ----------------------------------------------------------------------------

const log = (...args: unknown[]) => console.log("→", ...args);
const warn = (...args: unknown[]) => console.warn("⚠", ...args);
const ok = (...args: unknown[]) => console.log("✓", ...args);
const err = (...args: unknown[]) => console.error("✗", ...args);

const TYPE_PAIEMENT_MAP: Record<number, string> = {
  0: "especes",
  1: "cheque",
  2: "cb",
  3: "virement",
};

function chunked<T>(arr: T[], size: number): T[][] {
  const out: T[][] = [];
  for (let i = 0; i < arr.length; i += size) out.push(arr.slice(i, i + size));
  return out;
}

function normalizeDate(d: Date | string | null | undefined): string | null {
  if (!d) return null;
  if (d instanceof Date) return d.toISOString();
  return d;
}

// Insert par chunks via supabase.from().insert(). Plus efficace qu'une row à la fois.
async function bulkInsert<T extends Record<string, unknown>>(
  supa: SupabaseClient,
  table: string,
  rows: T[],
): Promise<void> {
  if (rows.length === 0) {
    log(`${table} : aucune ligne à insérer`);
    return;
  }
  const batchSize = 500;
  let inserted = 0;
  for (const batch of chunked(rows, batchSize)) {
    const { error } = await supa.from(table).insert(batch);
    if (error) {
      err(`${table} : ${error.message}`);
      err("première row du batch :", JSON.stringify(batch[0], null, 2).slice(0, 500));
      throw new Error(`bulkInsert ${table}: ${error.message}`);
    }
    inserted += batch.length;
    process.stdout.write(`  ${table}: ${inserted}/${rows.length}\r`);
  }
  process.stdout.write("\n");
  ok(`${table} : ${inserted} rows insérées`);
}

// ----------------------------------------------------------------------------
// Mappers — types source MSSQL → format insert Postgres (snake_case)
// ----------------------------------------------------------------------------

type MssqlAdresse = {
  Adresse_Ligne1: string | null;
  Adresse_Ligne2: string | null;
  Adresse_Ligne3: string | null;
  Adresse_CodePostal: string | null;
  Adresse_Ville: string | null;
  Adresse_Pays: string | null;
};

function mapAdresse(r: MssqlAdresse) {
  return {
    adresse_ligne1: r.Adresse_Ligne1,
    adresse_ligne2: r.Adresse_Ligne2,
    adresse_ligne3: r.Adresse_Ligne3,
    adresse_code_postal: r.Adresse_CodePostal,
    adresse_ville: r.Adresse_Ville,
    adresse_pays: r.Adresse_Pays,
  };
}

// ----------------------------------------------------------------------------
// Inspection (compteurs source)
// ----------------------------------------------------------------------------

const TABLES_SOURCE = [
  "Marques",
  "Modeles",
  "Entreprises",
  "Parametrages",
  "Fournisseurs",
  "Clients",
  "Voitures",
  "PieceVentes",
  "PieceVenteLignes",
  "PieceVenteServices",
  "PieceVentePaiements",
  "DocumentPieceVentes",
  "Salaries",
  "SalarieContrats",
  "SalarieIndisponibilites",
  "SalarieSalaires",
  "RendezVous",
  "Widgets",
  "WidgetUtilisateurs",
  "CompteUtilisateurs",
] as const;

const TABLES_TARGET = [
  "marques",
  "modeles",
  "entreprises",
  "parametrages",
  "fournisseurs",
  "clients",
  "voitures",
  "pieces_vente",
  "pieces_vente_lignes",
  "pieces_vente_services",
  "pieces_vente_paiements",
  "documents",
  "salaries",
  "salarie_contrats",
  "salarie_indisponibilites",
  "salarie_salaires",
  "rendez_vous",
  "widgets",
  "widgets_utilisateurs",
] as const;

async function countSource(pool: sql.ConnectionPool): Promise<Record<string, number>> {
  const out: Record<string, number> = {};
  for (const t of TABLES_SOURCE) {
    const r = await pool.request().query(`SELECT COUNT(*) AS n FROM [${t}]`);
    out[t] = r.recordset[0]?.n ?? 0;
  }
  return out;
}

async function countTarget(supa: SupabaseClient): Promise<Record<string, number>> {
  const out: Record<string, number> = {};
  for (const t of TABLES_TARGET) {
    const { count, error } = await supa.from(t).select("id", { count: "exact", head: true });
    if (error) {
      warn(`count ${t}: ${error.message}`);
      out[t] = -1;
    } else {
      out[t] = count ?? 0;
    }
  }
  return out;
}

// ----------------------------------------------------------------------------
// Étapes ETL
// ----------------------------------------------------------------------------

async function migrateMarques(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query("SELECT ID, Code, Libelle FROM Marques");
  const rows = r.recordset.map((s: { ID: number; Code: string; Libelle: string }) => ({
    id: s.ID,
    code: s.Code,
    libelle: s.Libelle,
  }));
  await bulkInsert(supa, "marques", rows);
}

async function migrateModeles(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query("SELECT ID, MarqueID, Libelle FROM Modeles");
  const rows = r.recordset.map(
    (s: { ID: number; MarqueID: number | null; Libelle: string }) => ({
      id: s.ID,
      marque_id: s.MarqueID,
      libelle: s.Libelle,
    }),
  );
  await bulkInsert(supa, "modeles", rows);
}

async function migrateParametrages(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, NomEntreprise, EmailEntreprise, TelephoneEntreprise, PortableEntreprise,
           Adresse_Ligne1, Adresse_Ligne2, Adresse_Ligne3,
           Adresse_CodePostal, Adresse_Ville, Adresse_Pays,
           TVA, MainDOeuvreMontantHoraire, EmailComptable,
           IsSavePieceDeVente, SIRET, CodeAPE, TVAIntraCommunautaire, LibelleBasDePage,
           EmailEnvoiSMTP, ProtocoleSMTP, PortSMTP, ServeurSMTP, EmailSMTP, PasswordSMTP,
           IsSSL, ObjectifAnnuel
    FROM Parametrages
  `);
  type Row = MssqlAdresse & {
    ID: number;
    NomEntreprise: string | null;
    EmailEntreprise: string | null;
    TelephoneEntreprise: string | null;
    PortableEntreprise: string | null;
    TVA: number;
    MainDOeuvreMontantHoraire: number;
    EmailComptable: string | null;
    IsSavePieceDeVente: boolean;
    SIRET: string | null;
    CodeAPE: string | null;
    TVAIntraCommunautaire: string | null;
    LibelleBasDePage: string | null;
    EmailEnvoiSMTP: string | null;
    ProtocoleSMTP: string | null;
    PortSMTP: number | null;
    ServeurSMTP: string | null;
    EmailSMTP: string | null;
    PasswordSMTP: string | null;
    IsSSL: boolean;
    ObjectifAnnuel: number | null;
  };
  const rows = r.recordset.map((s: Row) => ({
    id: s.ID,
    nom_entreprise: s.NomEntreprise,
    email_entreprise: s.EmailEntreprise,
    telephone_entreprise: s.TelephoneEntreprise,
    portable_entreprise: s.PortableEntreprise,
    ...mapAdresse(s),
    logo_path: null, // Logo varbinary non migré (NULL en source)
    entete_path: null,
    tva: s.TVA ?? 0.2,
    main_doeuvre_montant_horaire: s.MainDOeuvreMontantHoraire ?? 0,
    email_comptable: s.EmailComptable,
    is_save_piece_de_vente: s.IsSavePieceDeVente,
    siret: s.SIRET,
    code_ape: s.CodeAPE,
    tva_intra_communautaire: s.TVAIntraCommunautaire,
    libelle_bas_de_page: s.LibelleBasDePage,
    email_envoi_smtp: s.EmailEnvoiSMTP,
    protocole_smtp: s.ProtocoleSMTP,
    port_smtp: s.PortSMTP,
    serveur_smtp: s.ServeurSMTP,
    email_smtp: s.EmailSMTP,
    password_smtp: s.PasswordSMTP,
    is_ssl: s.IsSSL,
    objectif_annuel: s.ObjectifAnnuel,
  }));
  await bulkInsert(supa, "parametrages", rows);
}

async function migrateEntreprises(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, Nom, Adresse_Ligne1, Adresse_Ligne2, Adresse_Ligne3,
           Adresse_CodePostal, Adresse_Ville, Adresse_Pays,
           Siren, Siret, Commentaire, DateCreation, IsActif, ParametrageID
    FROM Entreprises
  `);
  type Row = MssqlAdresse & {
    ID: number;
    Nom: string;
    Siren: string | null;
    Siret: string | null;
    Commentaire: string | null;
    DateCreation: Date;
    IsActif: boolean;
    ParametrageID: number | null;
  };
  const rows = r.recordset.map((s: Row) => ({
    id: s.ID,
    nom: s.Nom,
    ...mapAdresse(s),
    siren: s.Siren,
    siret: s.Siret,
    commentaire: s.Commentaire,
    date_creation: normalizeDate(s.DateCreation),
    is_actif: s.IsActif,
    parametrage_id: s.ParametrageID,
  }));
  await bulkInsert(supa, "entreprises", rows);
}

async function migrateFournisseurs(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, Nom, Adresse_Ligne1, Adresse_Ligne2, Adresse_Ligne3,
           Adresse_CodePostal, Adresse_Ville, Adresse_Pays, Commentaire, DateSuppression
    FROM Fournisseurs
  `);
  type Row = MssqlAdresse & {
    ID: number;
    Nom: string;
    Commentaire: string | null;
    DateSuppression: Date | null;
  };
  const rows = r.recordset.map((s: Row) => ({
    id: s.ID,
    nom: s.Nom,
    ...mapAdresse(s),
    commentaire: s.Commentaire ?? "",
    date_suppression: normalizeDate(s.DateSuppression),
  }));
  await bulkInsert(supa, "fournisseurs", rows);
}

async function migrateClients(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, Code, Nom, Prenom,
           Adresse_Ligne1, Adresse_Ligne2, Adresse_Ligne3,
           Adresse_CodePostal, Adresse_Ville, Adresse_Pays,
           InformationsComplementaires, Telephone, Email,
           Remise, DateSuppression, IsProspect, EntrepriseID
    FROM Clients
  `);
  type Row = MssqlAdresse & {
    ID: number;
    Code: string | null;
    Nom: string;
    Prenom: string | null;
    InformationsComplementaires: string | null;
    Telephone: string | null;
    Email: string | null;
    Remise: number | null;
    DateSuppression: Date | null;
    IsProspect: boolean;
    EntrepriseID: number | null;
  };
  const rows = r.recordset.map((s: Row) => ({
    id: s.ID,
    code: s.Code,
    nom: s.Nom,
    prenom: s.Prenom,
    ...mapAdresse(s),
    informations_complementaires: s.InformationsComplementaires,
    telephone: s.Telephone,
    email: s.Email,
    remise: s.Remise,
    date_suppression: normalizeDate(s.DateSuppression),
    is_prospect: s.IsProspect,
    entreprise_id: s.EntrepriseID,
  }));
  await bulkInsert(supa, "clients", rows);
}

async function migrateVoitures(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, Immatriculation, ModeleID, ClientID, IsPrincipale, DateSuppression
    FROM Voitures
  `);
  const rows = r.recordset.map(
    (s: {
      ID: number;
      Immatriculation: string | null;
      ModeleID: number | null;
      ClientID: number | null;
      IsPrincipale: boolean;
      DateSuppression: Date | null;
    }) => ({
      id: s.ID,
      immatriculation: s.Immatriculation,
      modele_id: s.ModeleID,
      client_id: s.ClientID,
      is_principale: s.IsPrincipale,
      date_suppression: normalizeDate(s.DateSuppression),
    }),
  );
  await bulkInsert(supa, "voitures", rows);
}

async function migratePiecesVente(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, ClientID, VoitureID, RendezVousID,
           MainDOeuvreDuree, MainDOeuvreMontantHoraire,
           DateDevis, NumDevis, DateFacture, NumFacture, Kilometrage,
           IsFactureAnnule, DateHeureAnnulation, CommentaireAnnulation,
           IsValide, DateHeureValidation, IsEnvoyeComptable, IsDevisEnvoye,
           Remise, TotalTTC, MontantTVA, TotalHT, BeneficeTTC, ResteAPayer
    FROM PieceVentes
  `);
  type Row = {
    ID: number;
    ClientID: number | null;
    VoitureID: number | null;
    RendezVousID: number | null;
    MainDOeuvreDuree: number;
    MainDOeuvreMontantHoraire: number;
    DateDevis: Date | null;
    NumDevis: number | null;
    DateFacture: Date | null;
    NumFacture: number | null;
    Kilometrage: number;
    IsFactureAnnule: boolean;
    DateHeureAnnulation: Date | null;
    CommentaireAnnulation: string | null;
    IsValide: boolean;
    DateHeureValidation: Date | null;
    IsEnvoyeComptable: boolean;
    IsDevisEnvoye: boolean;
    Remise: number | null;
    TotalTTC: number;
    MontantTVA: number;
    TotalHT: number;
    BeneficeTTC: number;
    ResteAPayer: number;
  };
  const rows = r.recordset.map((s: Row) => ({
    id: s.ID,
    client_id: s.ClientID,
    voiture_id: s.VoitureID,
    rendez_vous_id: s.RendezVousID,
    main_doeuvre_duree: s.MainDOeuvreDuree,
    main_doeuvre_montant_horaire: s.MainDOeuvreMontantHoraire,
    date_devis: normalizeDate(s.DateDevis),
    num_devis: s.NumDevis,
    date_facture: normalizeDate(s.DateFacture),
    num_facture: s.NumFacture,
    kilometrage: s.Kilometrage,
    is_facture_annule: s.IsFactureAnnule,
    date_heure_annulation: normalizeDate(s.DateHeureAnnulation),
    commentaire_annulation: s.CommentaireAnnulation,
    is_valide: s.IsValide,
    date_heure_validation: normalizeDate(s.DateHeureValidation),
    is_envoye_comptable: s.IsEnvoyeComptable,
    is_devis_envoye: s.IsDevisEnvoye,
    remise: s.Remise,
    total_ttc: s.TotalTTC,
    montant_tva: s.MontantTVA,
    total_ht: s.TotalHT,
    benefice_ttc: s.BeneficeTTC,
    reste_a_payer: s.ResteAPayer,
  }));
  await bulkInsert(supa, "pieces_vente", rows);
}

async function migratePieceVenteLignes(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, PieceVenteID, FournisseurID, Libelle, Remise,
           PrixGarageHT, PrixGarageTTC, PrixClientHT, PrixClientTTC, Quantite
    FROM PieceVenteLignes
    WHERE PieceVenteID IS NOT NULL
  `);
  type Row = {
    ID: number;
    PieceVenteID: number;
    FournisseurID: number | null;
    Libelle: string | null;
    Remise: number;
    PrixGarageHT: number;
    PrixGarageTTC: number;
    PrixClientHT: number;
    PrixClientTTC: number;
    Quantite: number;
  };
  const rows = r.recordset.map((s: Row) => ({
    id: s.ID,
    piece_vente_id: s.PieceVenteID,
    fournisseur_id: s.FournisseurID,
    libelle: s.Libelle,
    remise: s.Remise,
    prix_garage_ht: s.PrixGarageHT,
    prix_garage_ttc: s.PrixGarageTTC,
    prix_client_ht: s.PrixClientHT,
    prix_client_ttc: s.PrixClientTTC,
    quantite: s.Quantite,
  }));
  await bulkInsert(supa, "pieces_vente_lignes", rows);
}

async function migratePieceVenteServices(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, PieceVenteID, Libelle, PrixClientHT, PrixClientTTC, Quantite
    FROM PieceVenteServices
    WHERE PieceVenteID IS NOT NULL
  `);
  const rows = r.recordset.map(
    (s: {
      ID: number;
      PieceVenteID: number;
      Libelle: string | null;
      PrixClientHT: number;
      PrixClientTTC: number;
      Quantite: number;
    }) => ({
      id: s.ID,
      piece_vente_id: s.PieceVenteID,
      libelle: s.Libelle,
      prix_client_ht: s.PrixClientHT,
      prix_client_ttc: s.PrixClientTTC,
      quantite: s.Quantite,
    }),
  );
  await bulkInsert(supa, "pieces_vente_services", rows);
}

async function migratePieceVentePaiements(pool: sql.ConnectionPool, supa: SupabaseClient) {
  const r = await pool.request().query(`
    SELECT ID, PieceVenteID, TypePaiement, [Date], Montant
    FROM PieceVentePaiements
    WHERE PieceVenteID IS NOT NULL
  `);
  const rows = r.recordset.map(
    (s: {
      ID: number;
      PieceVenteID: number;
      TypePaiement: number;
      Date: Date;
      Montant: number;
    }) => ({
      id: s.ID,
      piece_vente_id: s.PieceVenteID,
      type_paiement: TYPE_PAIEMENT_MAP[s.TypePaiement] ?? "especes",
      date: normalizeDate(s.Date),
      montant: s.Montant,
    }),
  );
  await bulkInsert(supa, "pieces_vente_paiements", rows);
}

// ----------------------------------------------------------------------------
// Pipeline principal
// ----------------------------------------------------------------------------

async function run() {
  log(`Mode : ${DRY_RUN ? "DRY RUN" : "RÉEL"}`);
  log(`Source : MSSQL ${process.env.MSSQL_HOST}:${process.env.MSSQL_PORT}/${process.env.MSSQL_DB}`);
  log(`Cible  : Supabase ${process.env.SUPABASE_URL}`);

  const pool = await connectMssql();
  ok("MSSQL connecté");

  const supa = connectSupabase();
  // Sanity check : compter les marques côté cible (devrait répondre 200)
  const sanity = await supa.from("marques").select("id", { count: "exact", head: true });
  if (sanity.error) {
    err(`Sanity check Supabase a échoué : ${sanity.error.message}`);
    err("Vérifiez SUPABASE_URL + SUPABASE_SERVICE_ROLE_KEY.");
    await pool.close();
    process.exit(1);
  }
  ok("Supabase connecté");

  const sourceCounts = await countSource(pool);
  log("Compteurs source :");
  for (const [t, n] of Object.entries(sourceCounts)) {
    console.log(`  ${t.padEnd(28)} ${n.toString().padStart(6)}`);
  }

  if (DRY_RUN) {
    const targetCounts = await countTarget(supa);
    log("Compteurs cible (avant ETL) :");
    for (const [t, n] of Object.entries(targetCounts)) {
      console.log(`  ${t.padEnd(28)} ${n.toString().padStart(6)}`);
    }
    ok("Dry run OK — pas de modification.");
    await pool.close();
    return;
  }

  // ---- TRUNCATE
  log("Vidage des tables cibles (etl_truncate_all_data) …");
  const trunc = await supa.rpc("etl_truncate_all_data");
  if (trunc.error) {
    err(`etl_truncate_all_data : ${trunc.error.message}`);
    err("Avez-vous appliqué db/etl_helpers.sql sur ce projet Supabase ?");
    await pool.close();
    process.exit(1);
  }
  ok("Tables cibles vidées.");

  // ---- INSERTIONS (ordre des FK)
  await migrateMarques(pool, supa);
  await migrateModeles(pool, supa);
  await migrateParametrages(pool, supa);
  await migrateEntreprises(pool, supa);
  await migrateFournisseurs(pool, supa);
  await migrateClients(pool, supa);
  await migrateVoitures(pool, supa);
  await migratePiecesVente(pool, supa);
  await migratePieceVenteLignes(pool, supa);
  await migratePieceVenteServices(pool, supa);
  await migratePieceVentePaiements(pool, supa);

  // ---- SETVAL sur toutes les sequences
  log("Réalignement des sequences (etl_reset_all_sequences) …");
  const reset = await supa.rpc("etl_reset_all_sequences");
  if (reset.error) {
    err(`etl_reset_all_sequences : ${reset.error.message}`);
  } else {
    ok("Sequences réalignées.");
  }

  // ---- VÉRIFICATION
  const targetCounts = await countTarget(supa);
  log("Comparaison source ↔ cible :");
  console.log(
    `  ${"table".padEnd(28)} ${"source".padStart(7)}  ${"cible".padStart(7)}  écart`,
  );
  const mappings: [keyof typeof sourceCounts, keyof typeof targetCounts][] = [
    ["Marques", "marques"],
    ["Modeles", "modeles"],
    ["Entreprises", "entreprises"],
    ["Parametrages", "parametrages"],
    ["Fournisseurs", "fournisseurs"],
    ["Clients", "clients"],
    ["Voitures", "voitures"],
    ["PieceVentes", "pieces_vente"],
    ["PieceVenteLignes", "pieces_vente_lignes"],
    ["PieceVenteServices", "pieces_vente_services"],
    ["PieceVentePaiements", "pieces_vente_paiements"],
  ];
  let mismatch = 0;
  for (const [s, t] of mappings) {
    const sv = sourceCounts[s] ?? 0;
    const tv = targetCounts[t] ?? 0;
    const diff = tv - sv;
    const flag = diff === 0 ? "✓" : "✗";
    console.log(
      `  ${(t as string).padEnd(28)} ${sv.toString().padStart(7)}  ${tv.toString().padStart(7)}  ${flag} ${diff}`,
    );
    if (diff !== 0) mismatch++;
  }
  if (mismatch === 0) {
    ok(`Migration OK — ${mappings.length} tables alignées.`);
  } else {
    warn(`${mismatch} tables présentent un écart. Inspecter manuellement.`);
  }

  await pool.close();
}

run().catch((e) => {
  err(e instanceof Error ? e.stack : String(e));
  process.exit(1);
});
