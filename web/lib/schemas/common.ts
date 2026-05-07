import { z } from "zod";

/**
 * Trim + null si chaîne vide. Pratique pour les champs optionnels d'un formulaire.
 */
export const optionalString = (max?: number) =>
  z
    .string()
    .trim()
    .max(max ?? 500)
    .transform((v) => (v === "" ? null : v))
    .nullable()
    .optional();

/**
 * Adresse : composant inline réutilisé sur plusieurs entités.
 */
export const adresseSchema = z.object({
  adresse_ligne1: optionalString(50),
  adresse_ligne2: optionalString(50),
  adresse_ligne3: optionalString(50),
  adresse_code_postal: optionalString(50),
  adresse_ville: optionalString(50),
  adresse_pays: optionalString(50),
});

export type AdresseInput = z.infer<typeof adresseSchema>;
