import { Pencil, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { listRendezVousAVenir, listRendezVousPasses } from "@/lib/data/agenda";
import { RdvDialog } from "./_components/rdv-dialog";
import { ConfirmActionButton } from "../_components/confirm-action-button";
import { createRendezVousAction, deleteRendezVousAction, updateRendezVousAction } from "./actions";

const fmtDateTime = (iso: string | null | undefined): string => {
  if (!iso) return "—";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return iso;
  return d.toLocaleString("fr-FR", {
    weekday: "short",
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });
};

export default async function AgendaPage() {
  const [aVenir, passes] = await Promise.all([listRendezVousAVenir(), listRendezVousPasses()]);

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Agenda</h1>
          <p className="text-sm text-neutral-500">{aVenir.length} rendez-vous à venir</p>
        </div>
        <RdvDialog
          trigger={
            <Button>
              <Plus className="mr-2 h-4 w-4" />
              Nouveau rendez-vous
            </Button>
          }
          title="Nouveau rendez-vous"
          action={createRendezVousAction}
        />
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">À venir</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {aVenir.length === 0 ? (
            <p className="text-sm text-neutral-500">Aucun rendez-vous à venir.</p>
          ) : (
            aVenir.map((rdv) => {
              const update = updateRendezVousAction.bind(null, rdv.id);
              const remove = deleteRendezVousAction.bind(null, rdv.id);
              return (
                <div
                  key={rdv.id}
                  className="flex items-start justify-between gap-3 rounded-md border p-3 hover:bg-neutral-50"
                >
                  <div className="flex-1">
                    <p className="font-medium">{rdv.sujet}</p>
                    <p className="text-sm text-neutral-500">
                      {fmtDateTime(rdv.date_heure_debut)}
                      {rdv.date_heure_fin && ` → ${fmtDateTime(rdv.date_heure_fin)}`}
                    </p>
                    {rdv.commentaire && (
                      <p className="mt-1 text-xs text-neutral-500">{rdv.commentaire}</p>
                    )}
                  </div>
                  <div className="flex items-center gap-1">
                    <RdvDialog
                      trigger={
                        <Button variant="ghost" size="icon" aria-label="Éditer">
                          <Pencil className="h-4 w-4" />
                        </Button>
                      }
                      title="Modifier le rendez-vous"
                      rdv={rdv}
                      action={update}
                    />
                    <ConfirmActionButton
                      label=""
                      confirmTitle="Supprimer ce rendez-vous ?"
                      confirmDescription="Action définitive."
                      confirmCta="Supprimer"
                      variant="ghost"
                      destructive
                      icon={<Trash2 className="h-4 w-4" />}
                      action={remove}
                    />
                  </div>
                </div>
              );
            })
          )}
        </CardContent>
      </Card>

      {passes.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="text-base text-neutral-500">Passés</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1">
            {passes.map((rdv) => (
              <div
                key={rdv.id}
                className="flex items-center justify-between rounded-md py-1 text-sm text-neutral-500"
              >
                <span>{rdv.sujet}</span>
                <span className="text-xs">{fmtDateTime(rdv.date_heure_debut)}</span>
              </div>
            ))}
          </CardContent>
        </Card>
      )}
    </div>
  );
}
