import Link from "next/link";
import { Plus, Search } from "@/components/icons";
import { listDevis } from "@/lib/data/pieces-vente";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { ListPagination } from "@/components/app/list-pagination";
import { DevisListTable } from "./_components/devis-list-table";

type SearchParams = Promise<{
  q?: string;
  envoye?: string;
  page?: string;
}>;

export default async function DevisListPage({ searchParams }: { searchParams: SearchParams }) {
  const sp = await searchParams;
  const q = sp.q?.toString() ?? "";
  const envoye = (sp.envoye as "tous" | "envoyes" | "non_envoyes" | undefined) ?? "tous";
  const page = Math.max(1, Number(sp.page ?? 1) || 1);

  const result = await listDevis({ q, envoye, page });

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Devis</h1>
          <p className="text-sm text-muted-foreground">
            {result.total} {result.total > 1 ? "résultats" : "résultat"}
          </p>
        </div>
        <Button asChild>
          <Link href="/devis/new">
            <Plus className="mr-2 h-4 w-4" />
            Nouveau devis
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
                placeholder="Rechercher par numéro de devis"
                className="pl-9"
                inputMode="numeric"
              />
            </div>
            <input type="hidden" name="envoye" value={envoye} />
            <Button type="submit" variant="secondary">
              Rechercher
            </Button>
          </form>

          <EnvoyeTabs current={envoye} q={q} />

          <DevisListTable rows={result.rows} />

          <ListPagination
            page={result.page}
            pageCount={result.pageCount}
            buildHref={(p) => {
              const qs = new URLSearchParams();
              if (q) qs.set("q", q);
              if (envoye !== "tous") qs.set("envoye", envoye);
              if (p > 1) qs.set("page", String(p));
              return `/devis${qs.toString() ? "?" + qs.toString() : ""}`;
            }}
          />
        </CardContent>
      </Card>
    </div>
  );
}

function EnvoyeTabs({ current, q }: { current: string; q?: string }) {
  const tabs = [
    { key: "tous", label: "Tous" },
    { key: "envoyes", label: "Envoyés" },
    { key: "non_envoyes", label: "Non envoyés" },
  ];
  const buildHref = (key: string) => {
    const qs = new URLSearchParams();
    if (q) qs.set("q", q);
    if (key !== "tous") qs.set("envoye", key);
    return `/devis${qs.toString() ? "?" + qs.toString() : ""}`;
  };
  return (
    <div className="flex items-center gap-1 rounded-md border bg-card p-1 text-sm">
      <span className="px-2 text-xs uppercase tracking-wider text-muted-foreground">Envoi</span>
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
