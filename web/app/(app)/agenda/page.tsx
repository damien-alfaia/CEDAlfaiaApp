import Link from "next/link";
import { Pencil, Plus, Trash2 } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { listRendezVousAVenir, listRendezVousMois, listRendezVousPasses } from "@/lib/data/agenda";
import { RdvDialog } from "./_components/rdv-dialog";
import { MonthCalendar } from "./_components/month-calendar";
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

type SearchParams = Promise<{ vue?: string; year?: string; month?: string }>;

export default async function AgendaPage({ searchParams }: { searchParams: SearchParams }) {
  const sp = await searchParams;
  const vue = sp.vue === "mois" ? "mois" : "liste";

  const now = new Date();
  const year = Number(sp.year ?? now.getFullYear());
  const month = Number(sp.month ?? now.getMonth() + 1);

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Agenda</h1>
          <p className="text-sm text-muted-foreground">
            {vue === "mois" ? "Vue calendrier mensuelle" : "Vue liste"}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <div className="flex items-center gap-1 rounded-md border bg-card p-1 text-sm">
            <Link
              href="/agenda"
              className={
                "rounded-md px-3 py-1.5 " +
                (vue === "liste"
                  ? "bg-primary text-primary-foreground"
                  : "text-foreground hover:bg-muted")
              }
            >
              Liste
            </Link>
            <Link
              href={`/agenda?vue=mois&year=${year}&month=${month.toString().padStart(2, "0")}`}
              className={
                "rounded-md px-3 py-1.5 " +
                (vue === "mois"
                  ? "bg-primary text-primary-foreground"
                  : "text-foreground hover:bg-muted")
              }
            >
              Mois
            </Link>
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
      </div>

      {vue === "mois" ? <ViewMois year={year} month={month} /> : <ViewListe />}
    </div>
  );
}

async function ViewMois({ year, month }: { year: number; month: number }) {
  const rdvs = await listRendezVousMois(year, month);
  return <MonthCalendar year={year} month={month} rdvs={rdvs} />;
}

async function ViewListe() {
  const [aVenir, passes] = await Promise.all([listRendezVousAVenir(), listRendezVousPasses()]);
  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle className="text-base">À venir ({aVenir.length})</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {aVenir.length === 0 ? (
            <p className="text-sm text-muted-foreground">Aucun rendez-vous à venir.</p>
          ) : (
            aVenir.map((rdv) => {
              const update = updateRendezVousAction.bind(null, rdv.id);
              const remove = deleteRendezVousAction.bind(null, rdv.id);
              return (
                <div
                  key={rdv.id}
                  className="flex items-start justify-between gap-3 rounded-md border p-3 hover:bg-muted/40"
                >
                  <div className="flex-1">
                    <p className="font-medium">{rdv.sujet}</p>
                    <p className="text-sm text-muted-foreground">
                      {fmtDateTime(rdv.date_heure_debut)}
                      {rdv.date_heure_fin && ` → ${fmtDateTime(rdv.date_heure_fin)}`}
                    </p>
                    {rdv.commentaire && (
                      <p className="mt-1 text-xs text-muted-foreground">{rdv.commentaire}</p>
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
            <CardTitle className="text-base text-muted-foreground">Passés (récents)</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1">
            {passes.map((rdv) => (
              <div
                key={rdv.id}
                className="flex items-center justify-between rounded-md py-1 text-sm text-muted-foreground"
              >
                <span>{rdv.sujet}</span>
                <span className="text-xs">{fmtDateTime(rdv.date_heure_debut)}</span>
              </div>
            ))}
          </CardContent>
        </Card>
      )}
    </>
  );
}
