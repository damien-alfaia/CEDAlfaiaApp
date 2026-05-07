import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, ArchiveRestore, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { FournisseurForm } from "../_components/fournisseur-form";
import {
  restoreFournisseurAction,
  softDeleteFournisseurAction,
  updateFournisseurAction,
} from "../actions";
import { getFournisseur } from "@/lib/data/fournisseurs";
import { ConfirmActionButton } from "../../_components/confirm-action-button";

export default async function FournisseurDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const fId = Number(id);
  if (!Number.isInteger(fId) || fId <= 0) notFound();
  const fournisseur = await getFournisseur(fId);
  if (!fournisseur) notFound();

  const update = updateFournisseurAction.bind(null, fId);

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <Button asChild variant="ghost" size="sm">
          <Link href="/fournisseurs">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour
          </Link>
        </Button>
        {fournisseur.date_suppression ? (
          <ConfirmActionButton
            label="Restaurer"
            confirmTitle="Restaurer ce fournisseur ?"
            confirmDescription="Le fournisseur redevient sélectionnable dans les lignes de devis/factures."
            confirmCta="Restaurer"
            variant="outline"
            icon={<ArchiveRestore className="mr-1 h-4 w-4" />}
            action={restoreFournisseurAction.bind(null, fId)}
          />
        ) : (
          <ConfirmActionButton
            label="Supprimer"
            confirmTitle="Supprimer ce fournisseur ?"
            confirmDescription="Suppression logique. Les lignes de devis/factures historiques restent."
            confirmCta="Supprimer"
            variant="outline"
            destructive
            icon={<Trash2 className="mr-1 h-4 w-4" />}
            action={softDeleteFournisseurAction.bind(null, fId)}
          />
        )}
      </div>
      <div className="flex flex-wrap items-center gap-3">
        <h1 className="text-2xl font-bold tracking-tight">{fournisseur.nom}</h1>
        {fournisseur.date_suppression ? (
          <Badge variant="secondary">Supprimé</Badge>
        ) : (
          <Badge>Actif</Badge>
        )}
      </div>
      <FournisseurForm fournisseur={fournisseur} action={update} />
    </div>
  );
}
