import "server-only";
import { PDFDocument } from "pdf-lib";

/**
 * Concat plusieurs buffers PDF en un seul. Utilisé par les routes batch
 * (devis/pdf-batch et factures/pdf-batch).
 */
export async function concatPdfs(buffers: Uint8Array[]): Promise<Uint8Array> {
  const merged = await PDFDocument.create();
  for (const buf of buffers) {
    const src = await PDFDocument.load(buf);
    const pages = await merged.copyPages(src, src.getPageIndices());
    pages.forEach((p) => merged.addPage(p));
  }
  return await merged.save();
}

/**
 * Parse `?ids=1,2,3` en tableau d'entiers, max 100 entries (DOS guard).
 */
export function parseIdsParam(raw: string | null): number[] {
  if (!raw) return [];
  const ids = raw
    .split(",")
    .map((s) => parseInt(s.trim(), 10))
    .filter((n) => Number.isInteger(n) && n > 0);
  return ids.slice(0, 100);
}
