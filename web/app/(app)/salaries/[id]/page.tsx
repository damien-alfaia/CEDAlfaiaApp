import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, Trash2 } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { requireRole } from "@/lib/auth";
import { getSalarie } from "@/lib/data/salaries";
import { SalarieForm } from "../_components/salarie-form";
import { deleteSalarieAction, updateSalarieAction } from "../actions";
import { ConfirmActionButton } from "../../_components/confirm-action-button";

export default async function SalarieDetailPage({ params }: { params: Promise<{ id: string }> }) {
  await requireRole(["super_admin", "administrateur"]);
  const { id } = await params;
  const sId = Number(id);
  if (!Number.isInteger(sId) || sId <= 0) notFound();
  const salarie = await getSalarie(sId);
  if (!salarie) notFound();

  const update = updateSalarieAction.bind(null, sId);
  const fullName = [salarie.prenom, salarie.nom].filter(Boolean).join(" ").trim() || "(sans nom)";

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <Button asChild variant="ghost" size="sm">
          <Link href="/salaries">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour
          </Link>
        </Button>
        <ConfirmActionButton
          label="Supprimer"
          confirmTitle="Supprimer ce salarié ?"
          confirmDescription="Suppression définitive. Les contrats / indisponibilités / salaires liés seront aussi supprimés."
          confirmCta="Supprimer"
          variant="outline"
          destructive
          icon={<Trash2 className="mr-1 h-4 w-4" />}
          action={deleteSalarieAction.bind(null, sId)}
        />
      </div>
      <h1 className="text-2xl font-bold tracking-tight">{fullName}</h1>

      <SalarieForm salarie={salarie} action={update} />

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Contrats &amp; indisponibilités</CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          Gestion détaillée des contrats, indisponibilités et bulletins de salaire à étendre
          ultérieurement (selon usage réel).
        </CardContent>
      </Card>
    </div>
  );
}
