import Link from "next/link";
import { ArrowRight, Send } from "@/components/icons";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { requireProfile, displayName } from "@/lib/auth";
import { fmtEuro } from "@/lib/format";
import { getKpis } from "@/lib/data/kpi";

export default async function DashboardPage() {
  const [profile, kpis] = await Promise.all([requireProfile(), getKpis()]);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Tableau de bord</h1>
        <p className="text-sm text-muted-foreground">Bienvenue, {displayName(profile)}.</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <KpiCard label="Devis du mois" value={kpis.nbDevisMois.toString()} href="/devis" />
        <KpiCard label="Factures du mois" value={kpis.nbFacturesMois.toString()} href="/factures" />
        <KpiCard label="CA TTC du mois" value={fmtEuro(kpis.caTtcMois)} href="/comptabilite" />
        <KpiCard
          label="Reste à payer"
          value={fmtEuro(kpis.resteAPayerTotal)}
          href="/comptabilite"
          emphasis={kpis.resteAPayerTotal > 0 ? "warn" : "ok"}
        />
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <ActionCard
          icon={<Send className="h-5 w-5" />}
          label="Devis non envoyés"
          count={kpis.nbDevisNonEnvoyes}
          href="/devis?envoye=non_envoyes"
        />
        <ActionCard
          icon={<ArrowRight className="h-5 w-5" />}
          label="Factures à envoyer comptable"
          count={kpis.nbFacturesAEnvoyerComptable}
          href="/factures?filter=a_envoyer"
        />
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Avancement de la migration</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p>✅ Phase 1 — Schéma Postgres</p>
          <p>✅ Phase 2 — Infra Next.js + Docker + Traefik</p>
          <p>✅ Phase 3 — Auth + layout</p>
          <p>✅ Phase 4 — Module Clients + Voitures</p>
          <p>✅ Phase 5 — Module Devis + Factures + PDFs</p>
          <p>✅ Phase 6 — Modules secondaires</p>
          <p className="text-muted-foreground">
            ⏳ Phase 7 — Migration des données SQL Server → Postgres
          </p>
          <p className="text-muted-foreground">⏳ Phase 8 — Cutover</p>
        </CardContent>
      </Card>
    </div>
  );
}

function KpiCard({
  label,
  value,
  href,
  emphasis,
}: {
  label: string;
  value: string;
  href?: string;
  emphasis?: "ok" | "warn";
}) {
  const color = emphasis === "ok" ? "text-green-700" : emphasis === "warn" ? "text-amber-700" : "";
  const inner = (
    <Card className={href ? "transition-colors hover:border-border" : ""}>
      <CardHeader className="pb-2">
        <CardTitle className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
          {label}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className={`text-2xl font-bold tabular-nums ${color}`}>{value}</div>
      </CardContent>
    </Card>
  );
  return href ? <Link href={href}>{inner}</Link> : inner;
}

function ActionCard({
  icon,
  label,
  count,
  href,
}: {
  icon: React.ReactNode;
  label: string;
  count: number;
  href: string;
}) {
  if (count === 0) {
    return (
      <Card className="opacity-60">
        <CardContent className="flex items-center gap-3 p-4">
          {icon}
          <div className="flex-1">
            <p className="text-sm font-medium">{label}</p>
            <p className="text-xs text-muted-foreground">Aucune action requise.</p>
          </div>
        </CardContent>
      </Card>
    );
  }
  return (
    <Link href={href}>
      <Card className="transition-colors hover:border-border">
        <CardContent className="flex items-center gap-3 p-4">
          {icon}
          <div className="flex-1">
            <p className="text-sm font-medium">{label}</p>
            <p className="text-xs text-muted-foreground">
              {count} {count > 1 ? "à traiter" : "à traiter"}
            </p>
          </div>
          <span className="rounded-md bg-amber-100 px-2 py-0.5 text-sm font-medium text-amber-800">
            {count}
          </span>
        </CardContent>
      </Card>
    </Link>
  );
}
