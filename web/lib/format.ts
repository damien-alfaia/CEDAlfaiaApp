/**
 * Formatters partagés (UI + PDFs).
 */

export function fmtEuro(n: number | null | undefined): string {
  const v = typeof n === "number" ? n : 0;
  return new Intl.NumberFormat("fr-FR", {
    style: "currency",
    currency: "EUR",
    minimumFractionDigits: 2,
  }).format(v);
}

export function fmtNumero(n: number | null | undefined): string {
  if (n === null || n === undefined) return "—";
  return n.toString().padStart(8, "0");
}

export function fmtDate(iso: string | null | undefined): string {
  if (!iso) return "—";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return iso;
  return d.toLocaleDateString("fr-FR");
}

export function fmtPercent(n: number | null | undefined): string {
  const v = typeof n === "number" ? n : 0;
  return new Intl.NumberFormat("fr-FR", {
    style: "percent",
    maximumFractionDigits: 2,
  }).format(v);
}

export const TYPE_PAIEMENT_LABEL: Record<string, string> = {
  especes: "Espèces",
  cheque: "Chèque",
  cb: "Carte bancaire",
  virement: "Virement",
};
