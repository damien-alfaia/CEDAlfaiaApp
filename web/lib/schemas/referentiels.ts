import { z } from "zod";

export const marqueWriteSchema = z.object({
  code: z.string().trim().min(1, "Code requis").max(5, "5 caractères max"),
  libelle: z.string().trim().min(1, "Libellé requis").max(500),
});
export type MarqueWriteInput = z.infer<typeof marqueWriteSchema>;

export function parseMarqueFormData(form: FormData): MarqueWriteInput {
  return marqueWriteSchema.parse({
    code: form.get("code")?.toString() ?? "",
    libelle: form.get("libelle")?.toString() ?? "",
  });
}

export const modeleWriteSchema = z.object({
  marque_id: z.coerce.number().int().positive("Marque requise"),
  libelle: z.string().trim().min(1, "Libellé requis").max(500),
});
export type ModeleWriteInput = z.infer<typeof modeleWriteSchema>;

export function parseModeleFormData(form: FormData): ModeleWriteInput {
  return modeleWriteSchema.parse({
    marque_id: form.get("marque_id"),
    libelle: form.get("libelle")?.toString() ?? "",
  });
}
