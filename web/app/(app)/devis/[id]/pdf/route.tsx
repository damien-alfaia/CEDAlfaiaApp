import { NextResponse } from "next/server";
import { renderToBuffer } from "@react-pdf/renderer";
import { requireProfile } from "@/lib/auth";
import { getParametrage, getPieceVenteFull } from "@/lib/data/pieces-vente";
import { PieceVentePdf } from "@/lib/pdf/piece-vente-pdf";

export async function GET(_req: Request, ctx: { params: Promise<{ id: string }> }) {
  await requireProfile();
  const { id } = await ctx.params;
  const devisId = Number(id);
  if (!Number.isInteger(devisId) || devisId <= 0) {
    return NextResponse.json({ error: "ID invalide" }, { status: 400 });
  }
  const piece = await getPieceVenteFull(devisId);
  if (!piece || piece.date_facture !== null) {
    return NextResponse.json({ error: "Devis introuvable" }, { status: 404 });
  }
  const parametrage = await getParametrage();
  const buffer = await renderToBuffer(
    <PieceVentePdf piece={piece} parametrage={parametrage} variant="devis" />,
  );
  const filename = `devis-${(piece.num_devis ?? piece.id).toString().padStart(8, "0")}.pdf`;
  return new NextResponse(new Uint8Array(buffer), {
    headers: {
      "Content-Type": "application/pdf",
      "Content-Disposition": `inline; filename="${filename}"`,
      "Cache-Control": "no-store",
    },
  });
}
