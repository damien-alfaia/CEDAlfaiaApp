import { z } from "zod";
import { adresseSchema, optionalString } from "./common";

/**
 * Schéma d'écriture pour un client (création + édition).
 *
 * - `nom` est requis (min 1 caractère après trim).
 * - `email` accepte vide ou une adresse valide (les clients pros n'en ont pas tjrs).
 * - `remise` 0..1 (ex: 0.05 pour 5%).
 * - `is_prospect` est explicite (true par défaut, l'app actuelle marque tout
 *   nouveau client comme prospect tant qu'il n'a pas de facture).
 */
export const clientWriteSchema = z
  .object({
    code: optionalString(256),
    nom: z.string().trim().min(1, "Le nom est obligatoire").max(256),
    prenom: optionalString(256),
    telephone: optionalString(100),
    email: z
      .union([z.string().trim().email("Email invalide"), z.literal("")])
      .transform((v) => (v === "" ? null : v))
      .nullable()
      .optional(),
    informations_complementaires: optionalString(5000),
    remise: z
      .union([z.number().min(0).max(1), z.literal("")])
      .transform((v) => (v === "" || v === undefined ? null : Number(v)))
      .nullable()
      .optional(),
    is_prospect: z.boolean().default(true),
  })
  .extend(adresseSchema.shape);

export type ClientWriteInput = z.infer<typeof clientWriteSchema>;

/**
 * Helpers pour parser FormData (Server Actions).
 */
export function parseClientFormData(form: FormData): ClientWriteInput {
  const raw = {
    code: form.get("code")?.toString() ?? "",
    nom: form.get("nom")?.toString() ?? "",
    prenom: form.get("prenom")?.toString() ?? "",
    telephone: form.get("telephone")?.toString() ?? "",
    email: form.get("email")?.toString() ?? "",
    informations_complementaires: form.get("informations_complementaires")?.toString() ?? "",
    remise: form.get("remise")?.toString() ?? "",
    is_prospect: form.get("is_prospect") === "on" || form.get("is_prospect") === "true",
    adresse_ligne1: form.get("adresse_ligne1")?.toString() ?? "",
    adresse_ligne2: form.get("adresse_ligne2")?.toString() ?? "",
    adresse_ligne3: form.get("adresse_ligne3")?.toString() ?? "",
    adresse_code_postal: form.get("adresse_code_postal")?.toString() ?? "",
    adresse_ville: form.get("adresse_ville")?.toString() ?? "",
    adresse_pays: form.get("adresse_pays")?.toString() ?? "",
  };
  // remise : convertir "0.05" string → 0.05 number AVANT parse
  const remiseStr = raw.remise.replace(",", ".");
  const remiseNum = remiseStr === "" ? "" : parseFloat(remiseStr);
  return clientWriteSchema.parse({
    ...raw,
    remise: Number.isNaN(remiseNum) ? "" : remiseNum,
  });
}
