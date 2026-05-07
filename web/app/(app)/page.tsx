import { requireProfile, displayName } from "@/lib/auth";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export default async function DashboardPage() {
  const profile = await requireProfile();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Tableau de bord</h1>
        <p className="text-sm text-neutral-500">Bienvenue, {displayName(profile)}.</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-xs font-medium uppercase tracking-wider text-neutral-500">
              Devis du mois
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-neutral-300">—</div>
            <p className="mt-1 text-xs text-neutral-400">arrive en Phase 5</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-xs font-medium uppercase tracking-wider text-neutral-500">
              Factures du mois
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-neutral-300">—</div>
            <p className="mt-1 text-xs text-neutral-400">arrive en Phase 5</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-xs font-medium uppercase tracking-wider text-neutral-500">
              CA TTC du mois
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-neutral-300">—</div>
            <p className="mt-1 text-xs text-neutral-400">arrive en Phase 6</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-xs font-medium uppercase tracking-wider text-neutral-500">
              Reste à payer
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-neutral-300">—</div>
            <p className="mt-1 text-xs text-neutral-400">arrive en Phase 6</p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Avancement de la migration</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p>✅ Phase 1 — Schéma Postgres</p>
          <p>✅ Phase 2 — Infra Next.js + Docker + Traefik</p>
          <p>✅ Phase 3 — Auth + layout</p>
          <p>✅ Phase 4 — Module Clients + Voitures</p>
          <p>✅ Phase 5 — Module Devis + Factures + PDFs</p>
          <p className="text-neutral-400">⏳ Phase 6 — Modules secondaires</p>
          <p className="text-neutral-400">
            ⏳ Phase 7 — Migration des données SQL Server → Postgres
          </p>
          <p className="text-neutral-400">⏳ Phase 8 — Cutover</p>
        </CardContent>
      </Card>
    </div>
  );
}
