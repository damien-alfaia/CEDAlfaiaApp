import { Badge } from "@/components/ui/badge";
import type { PieceVenteListRow } from "@/lib/db/types";

/**
 * Étiquette d'état d'une pièce de vente, calculée à partir de ses flags
 * (réplique l'arbre de décision de la liste de l'app actuelle).
 */
export function PieceVenteStateBadge({
  piece,
}: {
  piece: Pick<
    PieceVenteListRow,
    | "date_facture"
    | "is_facture_annule"
    | "is_valide"
    | "is_envoye_comptable"
    | "reste_a_payer"
    | "total_ttc"
  >;
}) {
  if (!piece.date_facture) return <Badge variant="outline">Devis</Badge>;
  if (piece.is_facture_annule) return <Badge variant="secondary">Annulée</Badge>;
  if (!piece.is_valide) return <Badge variant="outline">À valider</Badge>;
  if (piece.is_envoye_comptable) return <Badge>Envoyée comptable</Badge>;
  return <Badge>Validée</Badge>;
}

export function PaiementBadge({ reste, total }: { reste: number; total: number }) {
  if (total <= 0) return null;
  if (reste === 0) {
    return <Badge className="bg-green-100 text-green-800 hover:bg-green-100">Payé</Badge>;
  }
  if (reste === total) {
    return <Badge className="bg-red-100 text-red-800 hover:bg-red-100">À payer</Badge>;
  }
  return <Badge className="bg-amber-100 text-amber-800 hover:bg-amber-100">Partiel</Badge>;
}
