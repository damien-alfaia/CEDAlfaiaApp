import { NextResponse } from "next/server";
import { renderToBuffer } from "@react-pdf/renderer";
import { requireProfile } from "@/lib/auth";
import { getParametrage, getPieceVenteFull } from "@/lib/data/pieces-vente";
import { PieceVentePdf } from "@/lib/pdf/piece-vente-pdf";
import { concatPdfs, parseIdsParam } from "@/lib/pdf/batch";

export async function GET(req: Request) {
  await requireProfile();
  const url = new URL(req.url);
  const ids = parseIdsParam(url.searchParams.get("ids"));
  if (ids.length === 0) {
    return NextResponse.json({ error: "Aucun ID fourni" }, { status: 400 });
  }

  const parametrage = await getParametrage();
  const buffers: Uint8Array[] = [];
  const skipped: number[] = [];

  for (const id of ids) {
    const piece = await getPieceVenteFull(id);
    if (!piece || piece.date_facture !== null) {
      skipped.push(id);
      continue;
    }
    const buf = await renderToBuffer(
      <PieceVentePdf piece={piece} parametrage={parametrage} variant="devis" />,
    );
    buffers.push(new Uint8Array(buf));
  }

  if (buffers.length === 0) {
    return NextResponse.json({ error: "Aucun devis valide à imprimer", skipped }, { status: 404 });
  }

  const merged = await concatPdfs(buffers);
  const filename = `devis-lot-${new Date().toISOString().slice(0, 10)}-${buffers.length}.pdf`;
  return new NextResponse(new Uint8Array(merged), {
    headers: {
      "Content-Type": "application/pdf",
      "Content-Disposition": `inline; filename="${filename}"`,
      "Cache-Control": "no-store",
    },
  });
}
