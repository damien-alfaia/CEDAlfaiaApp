import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { requireRole } from "@/lib/auth";
import { fmtEuro } from "@/lib/format";
import { getBalanceAgee, getCaParMois, getKpis } from "@/lib/data/kpi";

const moisLabels = [
  "janv.",
  "févr.",
  "mars",
  "avr.",
  "mai",
  "juin",
  "juil.",
  "août",
  "sept.",
  "oct.",
  "nov.",
  "déc.",
];

function moisLabel(key: string): string {
  const [, mm] = key.split("-");
  return moisLabels[parseInt(mm, 10) - 1] ?? key;
}

export default async function ComptabilitePage() {
  await requireRole(["super_admin", "administrateur"]);

  const [kpis, caParMois, balance] = await Promise.all([
    getKpis(),
    getCaParMois(12),
    getBalanceAgee(),
  ]);

  const maxCa = Math.max(...caParMois.map((m) => m.caTtc), 1);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Comptabilité</h1>
        <p className="text-sm text-neutral-500">
          Vue agrégée de l&apos;activité (factures non annulées uniquement).
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <Kpi label="CA TTC mois" value={fmtEuro(kpis.caTtcMois)} />
        <Kpi label="Bénéfice TTC mois" value={fmtEuro(kpis.beneficeTtcMois)} />
        <Kpi label="CA TTC année" value={fmtEuro(kpis.caTtcAnnee)} />
        <Kpi label="Bénéfice TTC année" value={fmtEuro(kpis.beneficeTtcAnnee)} />
        <Kpi label="Reste à payer (ouvert)" value={fmtEuro(kpis.resteAPayerTotal)} />
        <Kpi
          label="Factures à envoyer comptable"
          value={kpis.nbFacturesAEnvoyerComptable.toString()}
          link={kpis.nbFacturesAEnvoyerComptable > 0 ? "/factures?filter=a_envoyer" : undefined}
        />
        <Kpi label="Factures du mois" value={kpis.nbFacturesMois.toString()} />
        <Kpi label="Devis du mois" value={kpis.nbDevisMois.toString()} />
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">CA TTC sur 12 mois</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-1">
            {caParMois.map((m) => (
              <div key={m.mois} className="flex items-center gap-3 text-sm">
                <div className="w-12 shrink-0 text-right text-neutral-500">{moisLabel(m.mois)}</div>
                <div className="flex-1 rounded bg-neutral-100">
                  <div
                    className="h-5 rounded bg-neutral-900"
                    style={{ width: `${(m.caTtc / maxCa) * 100}%` }}
                  />
                </div>
                <div className="w-28 shrink-0 text-right tabular-nums">{fmtEuro(m.caTtc)}</div>
                <div className="w-20 shrink-0 text-right tabular-nums text-neutral-500">
                  {m.nbFactures} fac.
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Balance âgée — clients à relancer</CardTitle>
        </CardHeader>
        <CardContent>
          {balance.length === 0 ? (
            <p className="text-sm text-neutral-500">Aucun reste à payer ouvert. 🎉</p>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Client</TableHead>
                    <TableHead className="text-right">Reste à payer</TableHead>
                    <TableHead className="text-right">Ancienneté max</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {balance.map((b) => (
                    <TableRow key={b.client_id}>
                      <TableCell>
                        <Link href={`/clients/${b.client_id}`} className="hover:underline">
                          {b.client_label}
                        </Link>
                      </TableCell>
                      <TableCell className="text-right tabular-nums">
                        {fmtEuro(b.reste_total)}
                      </TableCell>
                      <TableCell className="text-right tabular-nums">{b.jours_max} j</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

function Kpi({ label, value, link }: { label: string; value: string; link?: string }) {
  const inner = (
    <Card className={link ? "transition-colors hover:border-neutral-400" : ""}>
      <CardContent className="p-4">
        <p className="text-xs font-medium uppercase tracking-wider text-neutral-500">{label}</p>
        <p className="mt-1 text-2xl font-bold tabular-nums">{value}</p>
      </CardContent>
    </Card>
  );
  if (link) {
    return <Link href={link}>{inner}</Link>;
  }
  return inner;
}
