/**
 * Types TypeScript des tables Postgres (Supabase).
 *
 * Source : db/schema.sql (Phase 1).
 *
 * Convention : un type `*Row` (lecture, tous champs présents) et un `*Insert`
 * (écriture, champs auto-gérés en optionnel : id, created_at, updated_at).
 *
 * À regénérer via `supabase gen types typescript` dès que nous aurons
 * la Supabase CLI et un access token. En attendant, maintenu à la main.
 */

// =============================================================================
// Enums
// =============================================================================

export type UserRole = "super_admin" | "administrateur" | "secretaire" | "technicien";
export type TypePaiement = "especes" | "cheque" | "cb" | "virement";
export type TypeDocument = "devis" | "facture" | "bon_livraison_fournisseur" | "devis_fournisseur";
export type TypeIndisponibilite = "absence" | "conges" | "ecole" | "autre";

// =============================================================================
// Adresse (colonnes inline préfixées adresse_*)
// =============================================================================

export type Adresse = {
  adresse_ligne1: string | null;
  adresse_ligne2: string | null;
  adresse_ligne3: string | null;
  adresse_code_postal: string | null;
  adresse_ville: string | null;
  adresse_pays: string | null;
};

// =============================================================================
// Clients
// =============================================================================

export type ClientRow = Adresse & {
  id: number;
  code: string | null;
  nom: string;
  prenom: string | null;
  informations_complementaires: string | null;
  telephone: string | null;
  email: string | null;
  remise: number | null;
  date_suppression: string | null;
  is_prospect: boolean;
  entreprise_id: number | null;
  created_at: string;
  updated_at: string;
};

// =============================================================================
// Voitures
// =============================================================================

export type VoitureRow = {
  id: number;
  immatriculation: string | null;
  modele_id: number | null;
  client_id: number | null;
  is_principale: boolean;
  date_suppression: string | null;
  created_at: string;
  updated_at: string;
};

// Pour les listes : voiture jointe à modele + marque
export type VoitureWithModele = VoitureRow & {
  modele: {
    id: number;
    libelle: string;
    marque: { id: number; libelle: string } | null;
  } | null;
};

// =============================================================================
// Marques + modèles (référentiels)
// =============================================================================

export type MarqueRow = {
  id: number;
  code: string;
  libelle: string;
  created_at: string;
  updated_at: string;
};

export type ModeleRow = {
  id: number;
  marque_id: number | null;
  libelle: string;
  created_at: string;
  updated_at: string;
};

export type MarqueWithModeles = MarqueRow & {
  modeles: ModeleRow[];
};

// =============================================================================
// Pièces de vente (devis + factures)
// =============================================================================

export type PieceVenteRow = {
  id: number;
  client_id: number | null;
  voiture_id: number | null;
  rendez_vous_id: number | null;
  main_doeuvre_duree: number;
  main_doeuvre_montant_horaire: number;
  date_devis: string | null;
  num_devis: number | null;
  date_facture: string | null;
  num_facture: number | null;
  kilometrage: number;
  is_facture_annule: boolean;
  date_heure_annulation: string | null;
  commentaire_annulation: string | null;
  is_valide: boolean;
  date_heure_validation: string | null;
  is_envoye_comptable: boolean;
  is_devis_envoye: boolean;
  remise: number | null;
  total_ttc: number;
  montant_tva: number;
  total_ht: number;
  benefice_ttc: number;
  reste_a_payer: number;
  created_at: string;
  updated_at: string;
};

export type PieceVenteLigneRow = {
  id: number;
  piece_vente_id: number;
  fournisseur_id: number | null;
  libelle: string | null;
  remise: number;
  prix_garage_ht: number;
  prix_garage_ttc: number;
  prix_client_ht: number;
  prix_client_ttc: number;
  quantite: number;
};

export type PieceVenteServiceRow = {
  id: number;
  piece_vente_id: number;
  libelle: string | null;
  prix_client_ht: number;
  prix_client_ttc: number;
  quantite: number;
};

export type PieceVentePaiementRow = {
  id: number;
  piece_vente_id: number;
  type_paiement: TypePaiement;
  date: string;
  montant: number;
};

/** Vue détaillée d'une pièce de vente avec toutes ses sous-collections + relations. */
export type PieceVenteFull = PieceVenteRow & {
  client: {
    id: number;
    nom: string;
    prenom: string | null;
    telephone: string | null;
    email: string | null;
    code: string | null;
    adresse_ligne1: string | null;
    adresse_ligne2: string | null;
    adresse_ligne3: string | null;
    adresse_code_postal: string | null;
    adresse_ville: string | null;
    adresse_pays: string | null;
    remise: number | null;
  } | null;
  voiture: {
    id: number;
    immatriculation: string | null;
    modele: {
      id: number;
      libelle: string;
      marque: { id: number; libelle: string } | null;
    } | null;
  } | null;
  lignes: (PieceVenteLigneRow & {
    fournisseur: { id: number; nom: string } | null;
  })[];
  services: PieceVenteServiceRow[];
  paiements: PieceVentePaiementRow[];
};

/** Ligne de la liste (devis ou facture). */
export type PieceVenteListRow = PieceVenteRow & {
  client: {
    nom: string;
    prenom: string | null;
  } | null;
  voiture: {
    immatriculation: string | null;
    modele: {
      libelle: string;
      marque: { libelle: string } | null;
    } | null;
  } | null;
};

// =============================================================================
// Fournisseurs
// =============================================================================

export type FournisseurRow = Adresse & {
  id: number;
  nom: string;
  commentaire: string;
  date_suppression: string | null;
  created_at: string;
  updated_at: string;
};

// =============================================================================
// Salariés
// =============================================================================

export type SalarieRow = Adresse & {
  id: number;
  nom: string | null;
  prenom: string | null;
  date_naissance: string;
  telephone: string | null;
  portable: string | null;
  email: string | null;
  created_at: string;
  updated_at: string;
};

export type SalarieContratRow = {
  id: number;
  date_debut: string;
  date_fin: string | null;
  type_contrat: string | null;
  salarie_id: number | null;
  created_at: string;
  updated_at: string;
};

export type SalarieIndisponibiliteRow = {
  id: number;
  date_debut: string;
  date_fin: string;
  type_indisponibilite: TypeIndisponibilite;
  motif: string | null;
  salarie_contrat_id: number | null;
};

export type SalarieSalaireRow = {
  id: number;
  date_debut: string;
  date_fin: string;
  date_paiement: string;
  salaire_net: number;
  salarie_contrat_id: number | null;
};

// =============================================================================
// Rendez-vous
// =============================================================================

export type RendezVousRow = {
  id: number;
  sujet: string | null;
  date_heure_debut: string | null;
  date_heure_fin: string | null;
  duree: number | null;
  commentaire: string | null;
  created_at: string;
  updated_at: string;
};

// =============================================================================
// Parametrage
// =============================================================================

export type ParametrageRow = {
  id: number;
  nom_entreprise: string | null;
  email_entreprise: string | null;
  telephone_entreprise: string | null;
  portable_entreprise: string | null;
  adresse_ligne1: string | null;
  adresse_ligne2: string | null;
  adresse_ligne3: string | null;
  adresse_code_postal: string | null;
  adresse_ville: string | null;
  adresse_pays: string | null;
  logo_path: string | null;
  entete_path: string | null;
  tva: number;
  main_doeuvre_montant_horaire: number;
  email_comptable: string | null;
  siret: string | null;
  code_ape: string | null;
  tva_intra_communautaire: string | null;
  libelle_bas_de_page: string | null;
  objectif_annuel: number | null;
};
