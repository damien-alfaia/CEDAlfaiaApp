import { z } from "zod";
import { optionalString } from "./common";

export const rendezVousWriteSchema = z
  .object({
    sujet: z.string().trim().min(1, "Sujet requis").max(500),
    date_heure_debut: z.string().min(1, "Date/heure de début requise"),
    date_heure_fin: z.string().optional().nullable(),
    duree: z.coerce.number().int().min(0).optional().nullable(),
    commentaire: optionalString(2000),
  })
  .transform((v) => ({
    ...v,
    date_heure_fin: v.date_heure_fin === "" || v.date_heure_fin == null ? null : v.date_heure_fin,
    duree: v.duree == null || Number.isNaN(v.duree) ? null : v.duree,
  }));

export type RendezVousWriteInput = z.infer<typeof rendezVousWriteSchema>;

export function parseRendezVousFormData(form: FormData): RendezVousWriteInput {
  return rendezVousWriteSchema.parse({
    sujet: form.get("sujet")?.toString() ?? "",
    date_heure_debut: form.get("date_heure_debut")?.toString() ?? "",
    date_heure_fin: form.get("date_heure_fin")?.toString() ?? "",
    duree: form.get("duree")?.toString() ?? "",
    commentaire: form.get("commentaire")?.toString() ?? "",
  });
}
