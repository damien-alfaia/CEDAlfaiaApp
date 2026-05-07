import Link from "next/link";
import { ChevronLeft, ChevronRight } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import type { RendezVousRow } from "@/lib/db/types";

const JOURS = ["Lun", "Mar", "Mer", "Jeu", "Ven", "Sam", "Dim"];
const MOIS_LABELS = [
  "Janvier",
  "Février",
  "Mars",
  "Avril",
  "Mai",
  "Juin",
  "Juillet",
  "Août",
  "Septembre",
  "Octobre",
  "Novembre",
  "Décembre",
];

type Props = {
  year: number;
  month: number; // 1-12
  rdvs: RendezVousRow[];
};

function ymd(d: Date): string {
  const yyyy = d.getFullYear();
  const mm = (d.getMonth() + 1).toString().padStart(2, "0");
  const dd = d.getDate().toString().padStart(2, "0");
  return `${yyyy}-${mm}-${dd}`;
}

function fmtTime(iso: string | null): string {
  if (!iso) return "";
  const d = new Date(iso);
  return d.toLocaleTimeString("fr-FR", { hour: "2-digit", minute: "2-digit" });
}

export function MonthCalendar({ year, month, rdvs }: Props) {
  // Premier jour de la grille = lundi de la semaine du 1er du mois.
  const firstOfMonth = new Date(year, month - 1, 1);
  // 0=lundi : on ramène getDay() (0=dim..6=sam) → (1=lun..7=dim) puis -1
  const dow = (firstOfMonth.getDay() + 6) % 7; // 0=lun, 6=dim
  const gridStart = new Date(year, month - 1, 1 - dow);

  // 6 semaines × 7 jours = 42 cellules
  const days: Date[] = [];
  for (let i = 0; i < 42; i++) {
    days.push(new Date(gridStart.getFullYear(), gridStart.getMonth(), gridStart.getDate() + i));
  }

  // Indexation des RDV par jour (YYYY-MM-DD).
  const rdvsByDay = new Map<string, RendezVousRow[]>();
  for (const rdv of rdvs) {
    if (!rdv.date_heure_debut) continue;
    const k = ymd(new Date(rdv.date_heure_debut));
    const arr = rdvsByDay.get(k) ?? [];
    arr.push(rdv);
    rdvsByDay.set(k, arr);
  }

  const today = ymd(new Date());

  // Navigation mois précédent / suivant
  const prevMonth = month === 1 ? 12 : month - 1;
  const prevYear = month === 1 ? year - 1 : year;
  const nextMonth = month === 12 ? 1 : month + 1;
  const nextYear = month === 12 ? year + 1 : year;
  const buildHref = (y: number, m: number) =>
    `/agenda?vue=mois&year=${y}&month=${m.toString().padStart(2, "0")}`;

  return (
    <Card>
      <CardContent className="space-y-3 p-4">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold">
            {MOIS_LABELS[month - 1]} {year}
          </h2>
          <div className="flex gap-1">
            <Button asChild variant="outline" size="sm">
              <Link href={buildHref(prevYear, prevMonth)}>
                <ChevronLeft className="h-4 w-4" />
              </Link>
            </Button>
            <Button asChild variant="outline" size="sm">
              <Link
                href={`/agenda?vue=mois&year=${new Date().getFullYear()}&month=${(new Date().getMonth() + 1).toString().padStart(2, "0")}`}
              >
                Aujourd&apos;hui
              </Link>
            </Button>
            <Button asChild variant="outline" size="sm">
              <Link href={buildHref(nextYear, nextMonth)}>
                <ChevronRight className="h-4 w-4" />
              </Link>
            </Button>
          </div>
        </div>

        <div className="grid grid-cols-7 gap-1">
          {JOURS.map((j) => (
            <div
              key={j}
              className="px-2 py-1 text-center text-xs font-semibold uppercase tracking-wider text-muted-foreground"
            >
              {j}
            </div>
          ))}
          {days.map((d, i) => {
            const inMonth = d.getMonth() + 1 === month;
            const k = ymd(d);
            const dayRdvs = rdvsByDay.get(k) ?? [];
            const isToday = k === today;
            return (
              <div
                key={i}
                className={cn(
                  "min-h-[88px] rounded-md border border-border p-1.5 text-xs",
                  inMonth ? "bg-card" : "bg-muted/30 text-muted-foreground/60",
                  isToday && "ring-2 ring-primary",
                )}
              >
                <div className={cn("mb-1 flex justify-end font-medium", isToday && "text-primary")}>
                  {d.getDate()}
                </div>
                <div className="space-y-0.5">
                  {dayRdvs.slice(0, 3).map((rdv) => (
                    <div
                      key={rdv.id}
                      className="truncate rounded bg-accent/20 px-1.5 py-0.5 text-[11px] font-medium leading-tight"
                      title={`${fmtTime(rdv.date_heure_debut)} ${rdv.sujet ?? ""}`}
                    >
                      <span className="text-accent-foreground/80">
                        {fmtTime(rdv.date_heure_debut)}
                      </span>{" "}
                      {rdv.sujet}
                    </div>
                  ))}
                  {dayRdvs.length > 3 && (
                    <div className="text-[10px] text-muted-foreground">
                      +{dayRdvs.length - 3} autre{dayRdvs.length - 3 > 1 ? "s" : ""}
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </CardContent>
    </Card>
  );
}
