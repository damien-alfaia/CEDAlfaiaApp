import { z } from "zod";
import { adresseSchema, optionalString } from "./common";

export const salarieWriteSchema = z
  .object({
    nom: z.string().trim().min(1, "Nom requis").max(256),
    prenom: optionalString(256),
    date_naissance: z.string().min(1, "Date de naissance requise"),
    telephone: optionalString(50),
    portable: optionalString(50),
    email: z
      .union([z.string().trim().email("Email invalide"), z.literal("")])
      .transform((v) => (v === "" ? null : v))
      .nullable()
      .optional(),
  })
  .extend(adresseSchema.shape);

export type SalarieWriteInput = z.infer<typeof salarieWriteSchema>;

export function parseSalarieFormData(form: FormData): SalarieWriteInput {
  return salarieWriteSchema.parse({
    nom: form.get("nom")?.toString() ?? "",
    prenom: form.get("prenom")?.toString() ?? "",
    date_naissance: form.get("date_naissance")?.toString() ?? "",
    telephone: form.get("telephone")?.toString() ?? "",
    portable: form.get("portable")?.toString() ?? "",
    email: form.get("email")?.toString() ?? "",
    adresse_ligne1: form.get("adresse_ligne1")?.toString() ?? "",
    adresse_ligne2: form.get("adresse_ligne2")?.toString() ?? "",
    adresse_ligne3: form.get("adresse_ligne3")?.toString() ?? "",
    adresse_code_postal: form.get("adresse_code_postal")?.toString() ?? "",
    adresse_ville: form.get("adresse_ville")?.toString() ?? "",
    adresse_pays: form.get("adresse_pays")?.toString() ?? "",
  });
}
