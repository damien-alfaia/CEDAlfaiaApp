import { z } from "zod";

/**
 * Schémas Zod pour les pièces de vente (devis/facture) et sous-entités.
 *
 * La logique métier (réplique iso-fonctionnelle de l'app actuelle) :
 *  - Une PieceVente est un Devis si DateFacture est null.
 *  - Une PieceVente est une Facture si DateFacture est set.
 *  - L'utilisateur saisit prix HT et prix TTC séparément (la TVA n'est pas
 *    re-calculée en mémoire, juste sommée). C'est l'app actuelle qui fait ça.
 *  - Total HT = somme(lignes.HT * qte) + somme(services.HT * qte) + main_doeuvre,
 *    moins remise globale.
 *  - Total TTC = idem en TTC.
 *  - Bénéfice TTC = Total TTC - somme(lignes.prix_garage_ttc * qte).
 *  - Reste à payer = Total TTC - somme(paiements.montant).
 */

const number = z
  .union([z.number(), z.string()])
  .transform((v) => (typeof v === "number" ? v : parseFloat(v.replace(",", "."))))
  .refine((v) => !Number.isNaN(v), { message: "Nombre invalide" });

const optionalNumber = z
  .union([z.number(), z.string(), z.null(), z.undefined()])
  .transform((v) => {
    if (v === null || v === undefined || v === "") return null;
    const n = typeof v === "number" ? v : parseFloat(v.replace(",", "."));
    return Number.isNaN(n) ? null : n;
  })
  .nullable();

export const ligneSchema = z.object({
  id: z.coerce.number().optional(), // si présent, ligne existante
  fournisseur_id: z
    .union([z.coerce.number(), z.literal(""), z.null()])
    .transform((v) => (v === "" || v === null ? null : Number(v)))
    .nullable()
    .optional(),
  libelle: z.string().trim().min(1, "Libellé requis").max(500),
  quantite: z.coerce.number().int().min(1, "Quantité ≥ 1"),
  remise: number.default(0),
  prix_garage_ht: number.default(0),
  prix_garage_ttc: number.default(0),
  prix_client_ht: number.default(0),
  prix_client_ttc: number.default(0),
});
export type LigneInput = z.infer<typeof ligneSchema>;

export const serviceSchema = z.object({
  id: z.coerce.number().optional(),
  libelle: z.string().trim().min(1, "Libellé requis").max(500),
  quantite: z.coerce.number().int().min(1, "Quantité ≥ 1"),
  prix_client_ht: number.default(0),
  prix_client_ttc: number.default(0),
});
export type ServiceInput = z.infer<typeof serviceSchema>;

export const paiementSchema = z.object({
  id: z.coerce.number().optional(),
  type_paiement: z.enum(["especes", "cheque", "cb", "virement"]),
  date: z.string().min(1, "Date requise"),
  montant: number,
});
export type PaiementInput = z.infer<typeof paiementSchema>;

export const pieceVenteWriteSchema = z.object({
  type_piece: z.enum(["devis", "facture"]),
  client_id: z.coerce.number().int().positive("Client requis"),
  voiture_id: z.coerce.number().int().positive("Voiture requise"),
  date: z.string().min(1, "Date requise"),
  kilometrage: z.coerce.number().int().min(0).default(0),
  main_doeuvre_montant_horaire: number.default(0),
  main_doeuvre_duree: z.coerce.number().int().min(0).default(0),
  remise: optionalNumber, // 0..1 (ex: 0.05) ou null
  is_devis_envoye: z.boolean().default(false),
  lignes: z.array(ligneSchema).default([]),
  services: z.array(serviceSchema).default([]),
  paiements: z.array(paiementSchema).default([]),
});
export type PieceVenteWriteInput = z.infer<typeof pieceVenteWriteSchema>;

/**
 * Le formulaire client encode tout en JSON dans un input caché pour
 * gérer les listes imbriquées (lignes/services/paiements) sans gymnastique
 * de noms de champs. Le Server Action parse ce JSON.
 */
export function parsePieceVenteFormData(form: FormData): PieceVenteWriteInput {
  const payloadStr = form.get("payload")?.toString() ?? "{}";
  let raw: unknown;
  try {
    raw = JSON.parse(payloadStr);
  } catch {
    throw new Error("Payload JSON invalide");
  }
  return pieceVenteWriteSchema.parse(raw);
}

// =============================================================================
// Calculs côté serveur — utilisés à l'enregistrement et pour les PDFs
// =============================================================================

export type Totaux = {
  total_ht: number;
  total_ttc: number;
  montant_tva: number;
  benefice_ttc: number;
  reste_a_payer: number;
};

/**
 * Calculs iso-fonctionnels avec l'app actuelle :
 *   - main_doeuvre s'ajoute en HT comme en TTC (pas de TVA pour MO en historique).
 *   - lignes/services : HT et TTC ajoutés séparément (deux totaux indépendants).
 *   - remise globale : appliquée en pourcentage sur les deux totaux.
 *   - bénéfice TTC = total TTC client - total TTC garage (cost from supplier).
 */
export function calculerTotaux(
  input: Pick<
    PieceVenteWriteInput,
    | "main_doeuvre_montant_horaire"
    | "main_doeuvre_duree"
    | "remise"
    | "lignes"
    | "services"
    | "paiements"
  >,
): Totaux {
  const mo = input.main_doeuvre_montant_horaire * input.main_doeuvre_duree;
  let total_ht = mo;
  let total_ttc = mo;
  let total_garage_ttc = 0;

  for (const l of input.lignes) {
    total_ht += l.prix_client_ht * l.quantite;
    total_ttc += l.prix_client_ttc * l.quantite;
    total_garage_ttc += l.prix_garage_ttc * l.quantite;
  }
  for (const s of input.services) {
    total_ht += s.prix_client_ht * s.quantite;
    total_ttc += s.prix_client_ttc * s.quantite;
  }

  const remise = input.remise ?? 0;
  if (remise > 0) {
    total_ht *= 1 - remise;
    total_ttc *= 1 - remise;
  }

  const montant_tva = total_ttc - total_ht;
  const benefice_ttc = total_ttc - total_garage_ttc;
  const totalPaiements = input.paiements.reduce((acc, p) => acc + p.montant, 0);
  const reste_a_payer = total_ttc - totalPaiements;

  return {
    total_ht: round2(total_ht),
    total_ttc: round2(total_ttc),
    montant_tva: round2(montant_tva),
    benefice_ttc: round2(benefice_ttc),
    reste_a_payer: round2(reste_a_payer),
  };
}

function round2(n: number): number {
  return Math.round(n * 100) / 100;
}
