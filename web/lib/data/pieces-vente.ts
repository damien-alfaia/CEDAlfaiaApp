import "server-only";
import { createClient } from "@/lib/supabase/server";
import type {
  PieceVenteFull,
  PieceVenteListRow,
  ParametrageRow,
  FournisseurRow,
} from "@/lib/db/types";

const LIST_SELECT = `
  id, client_id, voiture_id, rendez_vous_id, main_doeuvre_duree, main_doeuvre_montant_horaire,
  date_devis, num_devis, date_facture, num_facture, kilometrage,
  is_facture_annule, date_heure_annulation, commentaire_annulation,
  is_valide, date_heure_validation, is_envoye_comptable, is_devis_envoye,
  remise, total_ttc, montant_tva, total_ht, benefice_ttc, reste_a_payer,
  created_at, updated_at,
  client:clients ( nom, prenom ),
  voiture:voitures ( immatriculation, modele:modeles ( libelle, marque:marques ( libelle ) ) )
`;

const FULL_SELECT = `
  id, client_id, voiture_id, rendez_vous_id, main_doeuvre_duree, main_doeuvre_montant_horaire,
  date_devis, num_devis, date_facture, num_facture, kilometrage,
  is_facture_annule, date_heure_annulation, commentaire_annulation,
  is_valide, date_heure_validation, is_envoye_comptable, is_devis_envoye,
  remise, total_ttc, montant_tva, total_ht, benefice_ttc, reste_a_payer,
  created_at, updated_at,
  client:clients ( id, nom, prenom, telephone, email, code,
    adresse_ligne1, adresse_ligne2, adresse_ligne3, adresse_code_postal, adresse_ville, adresse_pays, remise ),
  voiture:voitures ( id, immatriculation, modele:modeles ( id, libelle, marque:marques ( id, libelle ) ) ),
  lignes:pieces_vente_lignes ( *, fournisseur:fournisseurs ( id, nom ) ),
  services:pieces_vente_services ( * ),
  paiements:pieces_vente_paiements ( * )
`;

// =============================================================================
// Devis
// =============================================================================

export type DevisListParams = {
  q?: string;
  envoye?: "tous" | "envoyes" | "non_envoyes";
  page?: number;
  perPage?: number;
};

export type DevisListResult = {
  rows: PieceVenteListRow[];
  total: number;
  page: number;
  perPage: number;
  pageCount: number;
};

const DEFAULT_PER_PAGE = 25;

export async function listDevis(params: DevisListParams = {}): Promise<DevisListResult> {
  const supabase = await createClient();
  const page = Math.max(1, params.page ?? 1);
  const perPage = Math.min(100, Math.max(1, params.perPage ?? DEFAULT_PER_PAGE));

  let query = supabase
    .from("pieces_vente")
    .select(LIST_SELECT, { count: "exact" })
    .is("date_facture", null);

  if (params.envoye === "envoyes") query = query.eq("is_devis_envoye", true);
  if (params.envoye === "non_envoyes") query = query.eq("is_devis_envoye", false);

  const q = params.q?.trim();
  if (q) {
    const safe = q.replace(/[,()]/g, "");
    if (/^\d+$/.test(safe)) query = query.eq("num_devis", Number(safe));
  }

  const from = (page - 1) * perPage;
  const to = from + perPage - 1;
  query = query
    .order("date_devis", { ascending: false, nullsFirst: false })
    .order("num_devis", { ascending: false, nullsFirst: false })
    .range(from, to);

  const { data, error, count } = await query;
  if (error) throw new Error(`listDevis: ${error.message}`);
  const total = count ?? 0;
  return {
    rows: (data ?? []) as unknown as PieceVenteListRow[],
    total,
    page,
    perPage,
    pageCount: Math.max(1, Math.ceil(total / perPage)),
  };
}

// =============================================================================
// Factures
// =============================================================================

export type FacturesFilter = "actives" | "annulees" | "a_envoyer";

export type FacturesListParams = {
  filter?: FacturesFilter;
  q?: string;
  page?: number;
  perPage?: number;
};

export async function listFactures(params: FacturesListParams = {}): Promise<DevisListResult> {
  const supabase = await createClient();
  const page = Math.max(1, params.page ?? 1);
  const perPage = Math.min(100, Math.max(1, params.perPage ?? DEFAULT_PER_PAGE));
  const filter = params.filter ?? "actives";

  let query = supabase
    .from("pieces_vente")
    .select(LIST_SELECT, { count: "exact" })
    .not("date_facture", "is", null);

  if (filter === "actives") query = query.eq("is_facture_annule", false);
  if (filter === "annulees") query = query.eq("is_facture_annule", true);
  if (filter === "a_envoyer") {
    query = query
      .eq("is_facture_annule", false)
      .eq("is_valide", true)
      .eq("is_envoye_comptable", false);
  }

  const q = params.q?.trim();
  if (q && /^\d+$/.test(q)) query = query.eq("num_facture", Number(q));

  const from = (page - 1) * perPage;
  const to = from + perPage - 1;
  query = query
    .order("date_facture", { ascending: false, nullsFirst: false })
    .order("num_facture", { ascending: false, nullsFirst: false })
    .range(from, to);

  const { data, error, count } = await query;
  if (error) throw new Error(`listFactures: ${error.message}`);
  const total = count ?? 0;
  return {
    rows: (data ?? []) as unknown as PieceVenteListRow[],
    total,
    page,
    perPage,
    pageCount: Math.max(1, Math.ceil(total / perPage)),
  };
}

// =============================================================================
// Détail
// =============================================================================

export async function getPieceVenteFull(id: number): Promise<PieceVenteFull | null> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("pieces_vente")
    .select(FULL_SELECT)
    .eq("id", id)
    .maybeSingle();
  if (error) throw new Error(`getPieceVenteFull: ${error.message}`);
  return (data as unknown as PieceVenteFull | null) ?? null;
}

// =============================================================================
// Numérotation
// =============================================================================

/** Numérotation iso-fonctionnelle : YY000000 + auto-increment. */
async function getNextNum(field: "num_devis" | "num_facture"): Promise<number> {
  const supabase = await createClient();
  const yy = parseInt(new Date().getFullYear().toString().slice(2, 4), 10);
  const start = yy * 1_000_000;
  const { data, error } = await supabase
    .from("pieces_vente")
    .select(field)
    .gt(field, start)
    .order(field, { ascending: false })
    .limit(1);
  if (error) throw new Error(`getNextNum(${field}): ${error.message}`);
  const row = data?.[0] as Record<string, number | null> | undefined;
  const last = row?.[field];
  return (last ?? start) + 1;
}

export const getNextNumDevis = () => getNextNum("num_devis");
export const getNextNumFacture = () => getNextNum("num_facture");

// =============================================================================
// Référentiels nécessaires aux formulaires
// =============================================================================

export async function getParametrage(): Promise<ParametrageRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("parametrages")
    .select("*")
    .order("id")
    .limit(1)
    .maybeSingle();
  if (error) throw new Error(`getParametrage: ${error.message}`);
  return (data as unknown as ParametrageRow | null) ?? null;
}

export async function listFournisseursActifs(): Promise<FournisseurRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("fournisseurs")
    .select("*")
    .is("date_suppression", null)
    .order("nom");
  if (error) throw new Error(`listFournisseursActifs: ${error.message}`);
  return (data ?? []) as FournisseurRow[];
}

export async function listClientsForSelect(): Promise<
  { id: number; label: string; voitures: { id: number; label: string }[] }[]
> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("clients")
    .select(
      `id, nom, prenom, code,
       voitures ( id, immatriculation, date_suppression, modele:modeles ( libelle, marque:marques ( libelle ) ) )`,
    )
    .is("date_suppression", null)
    .order("nom");
  if (error) throw new Error(`listClientsForSelect: ${error.message}`);

  type Row = {
    id: number;
    nom: string;
    prenom: string | null;
    code: string | null;
    voitures: Array<{
      id: number;
      immatriculation: string | null;
      date_suppression: string | null;
      modele: { libelle: string; marque: { libelle: string } | null } | null;
    }>;
  };

  return ((data ?? []) as unknown as Row[]).map((c) => {
    const fullName = [c.prenom, c.nom].filter(Boolean).join(" ").trim() || c.nom;
    return {
      id: c.id,
      label: c.code ? `${fullName} (${c.code})` : fullName,
      voitures: c.voitures
        .filter((v) => v.date_suppression === null)
        .map((v) => ({
          id: v.id,
          label: [
            v.modele?.marque?.libelle,
            v.modele?.libelle,
            v.immatriculation ? `(${v.immatriculation})` : "",
          ]
            .filter(Boolean)
            .join(" ")
            .trim(),
        })),
    };
  });
}
