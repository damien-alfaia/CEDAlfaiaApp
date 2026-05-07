import { z } from "zod";
import { adresseSchema, optionalString } from "./common";

const optionalNumber = z
  .union([z.number(), z.string(), z.null(), z.undefined()])
  .transform((v) => {
    if (v === null || v === undefined || v === "") return null;
    const n = typeof v === "number" ? v : parseFloat(v.toString().replace(",", "."));
    return Number.isNaN(n) ? null : n;
  })
  .nullable();

const requiredNumber = z.union([z.number(), z.string()]).transform((v) => {
  const n = typeof v === "number" ? v : parseFloat(v.replace(",", "."));
  return Number.isNaN(n) ? 0 : n;
});

export const parametrageWriteSchema = z
  .object({
    nom_entreprise: optionalString(256),
    email_entreprise: optionalString(256),
    telephone_entreprise: optionalString(50),
    portable_entreprise: optionalString(50),
    tva: requiredNumber.default(0.2),
    main_doeuvre_montant_horaire: requiredNumber.default(0),
    email_comptable: optionalString(256),
    siret: optionalString(50),
    code_ape: optionalString(20),
    tva_intra_communautaire: optionalString(50),
    libelle_bas_de_page: optionalString(2000),
    objectif_annuel: optionalNumber,
    serveur_smtp: optionalString(256),
    port_smtp: z
      .union([z.coerce.number(), z.literal(""), z.null()])
      .transform((v) => (v === "" || v === null ? null : Number(v)))
      .nullable()
      .optional(),
    email_smtp: optionalString(256),
    is_ssl: z.boolean().default(false),
  })
  .extend(adresseSchema.shape);

export type ParametrageWriteInput = z.infer<typeof parametrageWriteSchema>;

export function parseParametrageFormData(form: FormData): ParametrageWriteInput {
  return parametrageWriteSchema.parse({
    nom_entreprise: form.get("nom_entreprise")?.toString() ?? "",
    email_entreprise: form.get("email_entreprise")?.toString() ?? "",
    telephone_entreprise: form.get("telephone_entreprise")?.toString() ?? "",
    portable_entreprise: form.get("portable_entreprise")?.toString() ?? "",
    tva: form.get("tva")?.toString() ?? "0",
    main_doeuvre_montant_horaire: form.get("main_doeuvre_montant_horaire")?.toString() ?? "0",
    email_comptable: form.get("email_comptable")?.toString() ?? "",
    siret: form.get("siret")?.toString() ?? "",
    code_ape: form.get("code_ape")?.toString() ?? "",
    tva_intra_communautaire: form.get("tva_intra_communautaire")?.toString() ?? "",
    libelle_bas_de_page: form.get("libelle_bas_de_page")?.toString() ?? "",
    objectif_annuel: form.get("objectif_annuel")?.toString() ?? "",
    serveur_smtp: form.get("serveur_smtp")?.toString() ?? "",
    port_smtp: form.get("port_smtp")?.toString() ?? "",
    email_smtp: form.get("email_smtp")?.toString() ?? "",
    is_ssl: form.get("is_ssl") === "on" || form.get("is_ssl") === "true",
    adresse_ligne1: form.get("adresse_ligne1")?.toString() ?? "",
    adresse_ligne2: form.get("adresse_ligne2")?.toString() ?? "",
    adresse_ligne3: form.get("adresse_ligne3")?.toString() ?? "",
    adresse_code_postal: form.get("adresse_code_postal")?.toString() ?? "",
    adresse_ville: form.get("adresse_ville")?.toString() ?? "",
    adresse_pays: form.get("adresse_pays")?.toString() ?? "",
  });
}
