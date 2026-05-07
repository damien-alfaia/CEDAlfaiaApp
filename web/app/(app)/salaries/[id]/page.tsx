import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, ChevronRight, Pencil, Plus, Trash2 } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { requireRole } from "@/lib/auth";
import { getSalarie } from "@/lib/data/salaries";
import { listContratsForSalarie } from "@/lib/data/salaries-sub";
import { fmtDate } from "@/lib/format";
import { SalarieForm } from "../_components/salarie-form";
import { ContratDialog } from "../_components/contrat-dialog";
import { deleteSalarieAction, updateSalarieAction } from "../actions";
import { createContratAction, deleteContratAction, updateContratAction } from "../_actions/sub";
import { ConfirmActionButton } from "../../_components/confirm-action-button";

export default async function SalarieDetailPage({ params }: { params: Promise<{ id: string }> }) {
  await requireRole(["super_admin", "administrateur"]);
  const { id } = await params;
  const sId = Number(id);
  if (!Number.isInteger(sId) || sId <= 0) notFound();
  const [salarie, contrats] = await Promise.all([getSalarie(sId), listContratsForSalarie(sId)]);
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
        <CardHeader className="flex flex-row items-center justify-between space-y-0">
          <CardTitle className="text-base">Contrats</CardTitle>
          <ContratDialog
            trigger={
              <Button size="sm">
                <Plus className="mr-1 h-4 w-4" />
                Ajouter un contrat
              </Button>
            }
            title="Nouveau contrat"
            salarieId={sId}
            action={createContratAction}
          />
        </CardHeader>
        <CardContent className="space-y-2">
          {contrats.length === 0 ? (
            <p className="text-sm text-muted-foreground">Aucun contrat.</p>
          ) : (
            contrats.map((c) => {
              const enCours = !c.date_fin || new Date(c.date_fin) > new Date();
              return (
                <div
                  key={c.id}
                  className="flex items-center gap-3 rounded-md border border-border p-3 hover:bg-muted/40"
                >
                  <div className="flex-1">
                    <div className="flex items-center gap-2">
                      <p className="font-medium">{c.type_contrat ?? "Contrat"}</p>
                      {enCours ? (
                        <Badge>En cours</Badge>
                      ) : (
                        <Badge variant="secondary">Terminé</Badge>
                      )}
                    </div>
                    <p className="text-sm text-muted-foreground">
                      Du {fmtDate(c.date_debut)} au {c.date_fin ? fmtDate(c.date_fin) : "—"}
                    </p>
                  </div>
                  <div className="flex items-center gap-1">
                    <ContratDialog
                      trigger={
                        <Button variant="ghost" size="icon" aria-label="Éditer">
                          <Pencil className="h-4 w-4" />
                        </Button>
                      }
                      title="Modifier le contrat"
                      salarieId={sId}
                      contrat={c}
                      action={updateContratAction.bind(null, c.id)}
                    />
                    <ConfirmActionButton
                      label=""
                      confirmTitle="Supprimer ce contrat ?"
                      confirmDescription="Les indisponibilités et bulletins liés seront aussi supprimés."
                      confirmCta="Supprimer"
                      variant="ghost"
                      destructive
                      icon={<Trash2 className="h-4 w-4" />}
                      action={deleteContratAction.bind(null, c.id, sId)}
                    />
                    <Button asChild variant="ghost" size="icon" aria-label="Détail">
                      <Link href={`/salaries/${sId}/contrats/${c.id}`}>
                        <ChevronRight className="h-4 w-4" />
                      </Link>
                    </Button>
                  </div>
                </div>
              );
            })
          )}
        </CardContent>
      </Card>
    </div>
  );
}
