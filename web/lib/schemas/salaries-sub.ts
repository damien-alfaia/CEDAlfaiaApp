import { z } from "zod";
import { optionalString } from "./common";

const optionalDate = z
  .union([z.string().min(1), z.literal(""), z.null()])
  .transform((v) => (v === "" || v === null ? null : v))
  .nullable()
  .optional();

const requiredNumber = z.union([z.number(), z.string()]).transform((v) => {
  const n = typeof v === "number" ? v : parseFloat(v.replace(",", "."));
  return Number.isNaN(n) ? 0 : n;
});

// =============================================================================
// Contrats
// =============================================================================

export const contratWriteSchema = z.object({
  salarie_id: z.coerce.number().int().positive(),
  date_debut: z.string().min(1, "Date de début requise"),
  date_fin: optionalDate,
  type_contrat: optionalString(100),
});
export type ContratWriteInput = z.infer<typeof contratWriteSchema>;

export function parseContratFormData(form: FormData): ContratWriteInput {
  return contratWriteSchema.parse({
    salarie_id: form.get("salarie_id"),
    date_debut: form.get("date_debut")?.toString() ?? "",
    date_fin: form.get("date_fin")?.toString() ?? "",
    type_contrat: form.get("type_contrat")?.toString() ?? "",
  });
}

// =============================================================================
// Indisponibilités
// =============================================================================

export const indisponibiliteWriteSchema = z.object({
  salarie_contrat_id: z.coerce.number().int().positive(),
  date_debut: z.string().min(1, "Date début requise"),
  date_fin: z.string().min(1, "Date fin requise"),
  type_indisponibilite: z.enum(["absence", "conges", "ecole", "autre"]),
  motif: optionalString(500),
});
export type IndisponibiliteWriteInput = z.infer<typeof indisponibiliteWriteSchema>;

export function parseIndisponibiliteFormData(form: FormData): IndisponibiliteWriteInput {
  return indisponibiliteWriteSchema.parse({
    salarie_contrat_id: form.get("salarie_contrat_id"),
    date_debut: form.get("date_debut")?.toString() ?? "",
    date_fin: form.get("date_fin")?.toString() ?? "",
    type_indisponibilite: form.get("type_indisponibilite")?.toString() ?? "absence",
    motif: form.get("motif")?.toString() ?? "",
  });
}

// =============================================================================
// Bulletins de salaire
// =============================================================================

export const salaireWriteSchema = z.object({
  salarie_contrat_id: z.coerce.number().int().positive(),
  date_debut: z.string().min(1, "Date début requise"),
  date_fin: z.string().min(1, "Date fin requise"),
  date_paiement: z.string().min(1, "Date paiement requise"),
  salaire_net: requiredNumber,
});
export type SalaireWriteInput = z.infer<typeof salaireWriteSchema>;

export function parseSalaireFormData(form: FormData): SalaireWriteInput {
  return salaireWriteSchema.parse({
    salarie_contrat_id: form.get("salarie_contrat_id"),
    date_debut: form.get("date_debut")?.toString() ?? "",
    date_fin: form.get("date_fin")?.toString() ?? "",
    date_paiement: form.get("date_paiement")?.toString() ?? "",
    salaire_net: form.get("salaire_net")?.toString() ?? "0",
  });
}
