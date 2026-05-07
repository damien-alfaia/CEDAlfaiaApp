import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, FileDown, Check, X, RotateCcw, Mail } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { PieceVenteForm } from "../../_components/piece-vente-form";
import {
  annulerFactureAction,
  envoyerComptableAction,
  repasserFactureAction,
  updatePieceVenteAction,
  validerFactureAction,
} from "../../_actions/pieces-vente";
import {
  getPieceVenteFull,
  getParametrage,
  listClientsForSelect,
  listFournisseursActifs,
} from "@/lib/data/pieces-vente";
import { fmtNumero, fmtDate } from "@/lib/format";
import { ConfirmActionButton } from "../../_components/confirm-action-button";
import { PaiementBadge, PieceVenteStateBadge } from "@/components/app/piece-vente-state";
import { AnnulerFactureDialog } from "./_components/annuler-dialog";
import type { PieceVenteWriteInput } from "@/lib/schemas/pieces-vente";

export default async function FactureDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const factId = Number(id);
  if (!Number.isInteger(factId) || factId <= 0) notFound();

  const [piece, clients, fournisseurs, parametrage] = await Promise.all([
    getPieceVenteFull(factId),
    listClientsForSelect(),
    listFournisseursActifs(),
    getParametrage(),
  ]);
  if (!piece) notFound();
  if (piece.date_facture === null) {
    return (
      <div className="space-y-3">
        <h1 className="text-xl font-bold">Pas encore une facture</h1>
        <p>
          Cette pièce n&apos;a pas encore été transformée en facture.{" "}
          <Link href={`/devis/${piece.id}`} className="underline">
            Voir le devis
          </Link>
          .
        </p>
      </div>
    );
  }

  const initial: Partial<PieceVenteWriteInput> = {
    type_piece: "facture",
    client_id: piece.client_id ?? 0,
    voiture_id: piece.voiture_id ?? 0,
    date: (piece.date_facture ?? "").slice(0, 10),
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
    paiements: piece.paiements.map((p) => ({
      id: p.id,
      type_paiement: p.type_paiement,
      date: p.date.slice(0, 10),
      montant: p.montant,
    })),
  };

  const update = updatePieceVenteAction.bind(null, factId);

  const emailComptable = parametrage?.email_comptable ?? "";
  const mailtoComptable = emailComptable
    ? `mailto:${emailComptable}?subject=${encodeURIComponent(`Facture n° ${fmtNumero(piece.num_facture)}`)}&body=${encodeURIComponent(
        `Bonjour,\n\nVeuillez trouver ci-joint la facture n° ${fmtNumero(piece.num_facture)} pour le client ${[piece.client?.prenom, piece.client?.nom].filter(Boolean).join(" ").trim()}.\n\nMontant : ${piece.total_ttc.toFixed(2)} €\n\nCordialement.`,
      )}`
    : null;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <Button asChild variant="ghost" size="sm">
          <Link href="/factures">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour aux factures
          </Link>
        </Button>
        <div className="flex flex-wrap items-center gap-2">
          <Button asChild variant="outline" size="sm">
            <a href={`/factures/${factId}/pdf`} target="_blank" rel="noopener noreferrer">
              <FileDown className="mr-1 h-4 w-4" />
              Imprimer PDF
            </a>
          </Button>
          {!piece.is_facture_annule && !piece.is_valide && (
            <ConfirmActionButton
              label="Valider"
              confirmTitle="Valider la facture ?"
              confirmDescription="Une facture validée ne devrait plus être modifiée."
              confirmCta="Valider"
              variant="default"
              icon={<Check className="mr-1 h-4 w-4" />}
              action={validerFactureAction.bind(null, factId)}
            />
          )}
          {!piece.is_facture_annule && (
            <AnnulerFactureDialog action={annulerFactureAction.bind(null, factId)} />
          )}
          {piece.is_facture_annule && (
            <ConfirmActionButton
              label="Repasser en facture"
              confirmTitle="Repasser cette facture en cours ?"
              confirmDescription="L'annulation sera retirée."
              confirmCta="Confirmer"
              variant="outline"
              icon={<RotateCcw className="mr-1 h-4 w-4" />}
              action={repasserFactureAction.bind(null, factId)}
            />
          )}
          {!piece.is_facture_annule && piece.is_valide && !piece.is_envoye_comptable && (
            <>
              {mailtoComptable && (
                <Button asChild variant="outline" size="sm">
                  <a href={mailtoComptable}>
                    <Mail className="mr-1 h-4 w-4" />
                    Email comptable
                  </a>
                </Button>
              )}
              <ConfirmActionButton
                label="Marquer envoyée comptable"
                confirmTitle="Marquer cette facture comme envoyée au comptable ?"
                confirmDescription="Pensez à avoir effectivement envoyé le PDF par mail avant."
                confirmCta="Confirmer"
                variant="default"
                action={envoyerComptableAction.bind(null, [factId])}
              />
            </>
          )}
        </div>
      </div>

      <Card>
        <CardContent className="flex flex-wrap items-center gap-6 p-4">
          <div className="flex-1 min-w-[160px]">
            <p className="text-sm text-muted-foreground">Facture n°</p>
            <p className="font-mono text-2xl font-bold">{fmtNumero(piece.num_facture)}</p>
          </div>
          <div>
            <p className="text-sm text-muted-foreground">Date</p>
            <p>{fmtDate(piece.date_facture)}</p>
          </div>
          <div>
            <p className="text-sm text-muted-foreground">État</p>
            <PieceVenteStateBadge piece={piece} />
          </div>
          <div>
            <p className="text-sm text-muted-foreground">Paiement</p>
            <PaiementBadge reste={piece.reste_a_payer} total={piece.total_ttc} />
          </div>
        </CardContent>
      </Card>

      {piece.is_facture_annule && piece.commentaire_annulation && (
        <Card className="border-amber-300 bg-amber-50">
          <CardContent className="p-4 text-sm">
            <p className="font-semibold text-amber-900">Motif d&apos;annulation</p>
            <p className="text-amber-800">{piece.commentaire_annulation}</p>
          </CardContent>
        </Card>
      )}

      <PieceVenteForm
        mode="facture"
        clients={clients}
        fournisseurs={fournisseurs.map((f) => ({ id: f.id, label: f.nom }))}
        initial={initial}
        action={update}
        submitLabel="Enregistrer"
      />
    </div>
  );
}
