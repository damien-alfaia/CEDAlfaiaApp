import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, FileDown, ArrowRight } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { PieceVenteForm } from "../../_components/piece-vente-form";
import {
  deleteDevisAction,
  genererFactureAction,
  updatePieceVenteAction,
} from "../../_actions/pieces-vente";
import {
  getPieceVenteFull,
  listClientsForSelect,
  listFournisseursActifs,
} from "@/lib/data/pieces-vente";
import { fmtNumero, fmtDate } from "@/lib/format";
import { ConfirmActionButton } from "../../_components/confirm-action-button";
import type { PieceVenteWriteInput } from "@/lib/schemas/pieces-vente";

export default async function DevisDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const devisId = Number(id);
  if (!Number.isInteger(devisId) || devisId <= 0) notFound();

  const [piece, clients, fournisseurs] = await Promise.all([
    getPieceVenteFull(devisId),
    listClientsForSelect(),
    listFournisseursActifs(),
  ]);
  if (!piece) notFound();
  if (piece.date_facture !== null) {
    // déjà transformé en facture → rediriger pour cohérence
    return (
      <div className="space-y-3">
        <h1 className="text-xl font-bold">Devis transformé en facture</h1>
        <p>
          Cette pièce est désormais une facture (n° {fmtNumero(piece.num_facture)}).{" "}
          <Link href={`/factures/${piece.id}`} className="underline">
            Voir la facture
          </Link>
          .
        </p>
      </div>
    );
  }

  const initial: Partial<PieceVenteWriteInput> = {
    type_piece: "devis",
    client_id: piece.client_id ?? 0,
    voiture_id: piece.voiture_id ?? 0,
    date: (piece.date_devis ?? "").slice(0, 10),
    kilometrage: piece.kilometrage,
    main_doeuvre_montant_horaire: piece.main_doeuvre_montant_horaire,
    main_doeuvre_duree: piece.main_doeuvre_duree,
    remise: piece.remise,
    is_devis_envoye: piece.is_devis_envoye,
    lignes: piece.lignes.map((l) => ({
      id: l.id,
      fournisseur_id: l.fournisseur_id,
      libelle: l.libelle ?? "",
      quantite: l.quantite,
      remise: l.remise,
      prix_garage_ht: l.prix_garage_ht,
      prix_garage_ttc: l.prix_garage_ttc,
      prix_client_ht: l.prix_client_ht,
      prix_client_ttc: l.prix_client_ttc,
    })),
    services: piece.services.map((s) => ({
      id: s.id,
      libelle: s.libelle ?? "",
      quantite: s.quantite,
      prix_client_ht: s.prix_client_ht,
      prix_client_ttc: s.prix_client_ttc,
    })),
    paiements: [],
  };

  const update = updatePieceVenteAction.bind(null, devisId);

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <Button asChild variant="ghost" size="sm">
          <Link href="/devis">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour aux devis
          </Link>
        </Button>
        <div className="flex flex-wrap items-center gap-2">
          <Button asChild variant="outline" size="sm">
            <a href={`/devis/${devisId}/pdf`} target="_blank" rel="noopener noreferrer">
              <FileDown className="mr-1 h-4 w-4" />
              Imprimer PDF
            </a>
          </Button>
          <ConfirmActionButton
            label="Générer en facture"
            confirmTitle="Transformer ce devis en facture ?"
            confirmDescription="Un numéro de facture sera attribué et la pièce ne pourra plus être modifiée comme devis."
            confirmCta="Confirmer"
            variant="default"
            icon={<ArrowRight className="mr-1 h-4 w-4" />}
            action={genererFactureAction.bind(null, devisId)}
          />
          <ConfirmActionButton
            label="Supprimer"
            confirmTitle="Supprimer le devis ?"
            confirmDescription="Suppression définitive (le devis n'est pas encore une facture)."
            confirmCta="Supprimer"
            variant="outline"
            destructive
            action={deleteDevisAction.bind(null, devisId)}
          />
        </div>
      </div>

      <Card>
        <CardContent className="flex flex-wrap items-center gap-4 p-4">
          <div className="flex-1">
            <p className="text-sm text-muted-foreground">Devis n°</p>
            <p className="font-mono text-2xl font-bold">{fmtNumero(piece.num_devis)}</p>
          </div>
          <div className="text-right">
            <p className="text-sm text-muted-foreground">Date</p>
            <p>{fmtDate(piece.date_devis)}</p>
          </div>
        </CardContent>
      </Card>

      <PieceVenteForm
        mode="devis"
        clients={clients}
        fournisseurs={fournisseurs.map((f) => ({ id: f.id, label: f.nom }))}
        initial={initial}
        action={update}
        submitLabel="Enregistrer"
      />
    </div>
  );
}
