import { z } from "zod";
import { adresseSchema, optionalString } from "./common";

export const fournisseurWriteSchema = z
  .object({
    nom: z.string().trim().min(1, "Le nom est obligatoire").max(256),
    commentaire: optionalString(5000),
  })
  .extend(adresseSchema.shape);

export type FournisseurWriteInput = z.infer<typeof fournisseurWriteSchema>;

export function parseFournisseurFormData(form: FormData): FournisseurWriteInput {
  return fournisseurWriteSchema.parse({
    nom: form.get("nom")?.toString() ?? "",
    commentaire: form.get("commentaire")?.toString() ?? "",
    adresse_ligne1: form.get("adresse_ligne1")?.toString() ?? "",
    adresse_ligne2: form.get("adresse_ligne2")?.toString() ?? "",
    adresse_ligne3: form.get("adresse_ligne3")?.toString() ?? "",
    adresse_code_postal: form.get("adresse_code_postal")?.toString() ?? "",
    adresse_ville: form.get("adresse_ville")?.toString() ?? "",
    adresse_pays: form.get("adresse_pays")?.toString() ?? "",
  });
}
