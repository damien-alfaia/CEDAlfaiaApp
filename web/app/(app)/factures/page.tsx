import Link from "next/link";
import { Plus, Search } from "@/components/icons";
import { listFactures, type FacturesFilter } from "@/lib/data/pieces-vente";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { fmtDate, fmtEuro, fmtNumero } from "@/lib/format";
import { ListPagination } from "@/components/app/list-pagination";
import { PaiementBadge, PieceVenteStateBadge } from "@/components/app/piece-vente-state";

type SearchParams = Promise<{
  q?: string;
  filter?: string;
  page?: string;
}>;

export default async function FacturesListPage({ searchParams }: { searchParams: SearchParams }) {
  const sp = await searchParams;
  const q = sp.q?.toString() ?? "";
  const filter = (sp.filter as FacturesFilter) ?? "actives";
  const page = Math.max(1, Number(sp.page ?? 1) || 1);

  const result = await listFactures({ q, filter, page });

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Factures</h1>
          <p className="text-sm text-muted-foreground">
            {result.total} {result.total > 1 ? "résultats" : "résultat"}
          </p>
        </div>
        <Button asChild>
          <Link href="/factures/new">
            <Plus className="mr-2 h-4 w-4" />
            Nouvelle facture
          </Link>
        </Button>
      </div>

      <Card>
        <CardContent className="space-y-4 p-4">
          <form className="flex flex-wrap items-center gap-2">
            <div className="relative flex-1 min-w-[200px]">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                name="q"
                defaultValue={q}
                placeholder="Rechercher par numéro de facture"
                className="pl-9"
                inputMode="numeric"
              />
            </div>
            <input type="hidden" name="filter" value={filter} />
            <Button type="submit" variant="secondary">
              Rechercher
            </Button>
          </form>

          <FilterTabs current={filter} q={q} />

          {result.rows.length === 0 ? (
            <div className="rounded-md border border-dashed p-8 text-center text-sm text-muted-foreground">
              Aucune facture à afficher.
            </div>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>N°</TableHead>
                    <TableHead className="hidden sm:table-cell">Date</TableHead>
                    <TableHead>Client</TableHead>
                    <TableHead className="hidden md:table-cell">Voiture</TableHead>
                    <TableHead className="text-right">Total TTC</TableHead>
                    <TableHead className="text-right hidden lg:table-cell">Reste</TableHead>
                    <TableHead>État</TableHead>
                    <TableHead>Paiement</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {result.rows.map((d) => {
                    const fullName = [d.client?.prenom, d.client?.nom]
                      .filter(Boolean)
                      .join(" ")
                      .trim();
                    const voiture =
                      d.voiture?.modele?.marque?.libelle && d.voiture?.modele?.libelle
                        ? `${d.voiture.modele.marque.libelle} ${d.voiture.modele.libelle}`
                        : "—";
                    return (
                      <TableRow key={d.id}>
                        <TableCell className="font-mono">
                          <Link href={`/factures/${d.id}`} className="hover:underline">
                            {fmtNumero(d.num_facture)}
                          </Link>
                        </TableCell>
                        <TableCell className="hidden sm:table-cell">
                          {fmtDate(d.date_facture)}
                        </TableCell>
                        <TableCell>{fullName || "—"}</TableCell>
                        <TableCell className="hidden md:table-cell">
                          {voiture}
                          {d.voiture?.immatriculation && (
                            <span className="ml-1 font-mono text-xs text-muted-foreground">
                              {d.voiture.immatriculation}
                            </span>
                          )}
                        </TableCell>
                        <TableCell className="text-right tabular-nums">
                          {fmtEuro(d.total_ttc)}
                        </TableCell>
                        <TableCell className="hidden lg:table-cell text-right tabular-nums">
                          {fmtEuro(d.reste_a_payer)}
                        </TableCell>
                        <TableCell>
                          <PieceVenteStateBadge piece={d} />
                        </TableCell>
                        <TableCell>
                          <PaiementBadge reste={d.reste_a_payer} total={d.total_ttc} />
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          )}

          <ListPagination
            page={result.page}
            pageCount={result.pageCount}
            buildHref={(p) => {
              const qs = new URLSearchParams();
              if (q) qs.set("q", q);
              if (filter !== "actives") qs.set("filter", filter);
              if (p > 1) qs.set("page", String(p));
              return `/factures${qs.toString() ? "?" + qs.toString() : ""}`;
            }}
          />
        </CardContent>
      </Card>
    </div>
  );
}

function FilterTabs({ current, q }: { current: FacturesFilter; q?: string }) {
  const tabs: { key: FacturesFilter; label: string }[] = [
    { key: "actives", label: "Actives" },
    { key: "a_envoyer", label: "À envoyer comptable" },
    { key: "annulees", label: "Annulées" },
  ];
  const buildHref = (key: FacturesFilter) => {
    const qs = new URLSearchParams();
    if (q) qs.set("q", q);
    if (key !== "actives") qs.set("filter", key);
    return `/factures${qs.toString() ? "?" + qs.toString() : ""}`;
  };
  return (
    <div className="flex flex-wrap items-center gap-1 rounded-md border bg-card p-1 text-sm">
      <span className="px-2 text-xs uppercase tracking-wider text-muted-foreground">Filtre</span>
      {tabs.map((t) => (
        <Link
          key={t.key}
          href={buildHref(t.key)}
          className={
            "rounded-md px-3 py-1.5 " +
            (current === t.key
              ? "bg-primary text-primary-foreground"
              : "text-foreground hover:bg-muted")
          }
        >
          {t.label}
        </Link>
      ))}
    </div>
  );
}
