import { z } from "zod";
import { optionalString } from "./common";

export const voitureWriteSchema = z.object({
  immatriculation: optionalString(20),
  modele_id: z.coerce.number().int().positive("Modèle obligatoire"),
  client_id: z.coerce.number().int().positive(),
  is_principale: z.boolean().default(false),
});

export type VoitureWriteInput = z.infer<typeof voitureWriteSchema>;

export function parseVoitureFormData(form: FormData): VoitureWriteInput {
  return voitureWriteSchema.parse({
    immatriculation: form.get("immatriculation")?.toString() ?? "",
    modele_id: form.get("modele_id"),
    client_id: form.get("client_id"),
    is_principale: form.get("is_principale") === "on" || form.get("is_principale") === "true",
  });
}
