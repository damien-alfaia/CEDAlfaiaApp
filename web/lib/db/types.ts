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
