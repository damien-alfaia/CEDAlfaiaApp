import Link from "next/link";
import { Plus, Search, Check, X } from "@/components/icons";
import { listDevis } from "@/lib/data/pieces-vente";
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
          <p className="text-sm text-neutral-500">
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
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-neutral-400" />
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

          {result.rows.length === 0 ? (
            <div className="rounded-md border border-dashed p-8 text-center text-sm text-neutral-500">
              Aucun devis à afficher.
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
                    <TableHead className="text-center">Envoyé</TableHead>
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
                          <Link href={`/devis/${d.id}`} className="hover:underline">
                            {fmtNumero(d.num_devis)}
                          </Link>
                        </TableCell>
                        <TableCell className="hidden sm:table-cell">
                          {fmtDate(d.date_devis)}
                        </TableCell>
                        <TableCell>{fullName || "—"}</TableCell>
                        <TableCell className="hidden md:table-cell">
                          {voiture}
                          {d.voiture?.immatriculation && (
                            <span className="ml-1 font-mono text-xs text-neutral-400">
                              {d.voiture.immatriculation}
                            </span>
                          )}
                        </TableCell>
                        <TableCell className="text-right tabular-nums">
                          {fmtEuro(d.total_ttc)}
                        </TableCell>
                        <TableCell className="text-center">
                          {d.is_devis_envoye ? (
                            <Check className="mx-auto h-4 w-4 text-green-600" />
                          ) : (
                            <X className="mx-auto h-4 w-4 text-neutral-300" />
                          )}
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
    <div className="flex items-center gap-1 rounded-md border bg-white p-1 text-sm">
      <span className="px-2 text-xs uppercase tracking-wider text-neutral-400">Envoi</span>
      {tabs.map((t) => (
        <Link
          key={t.key}
          href={buildHref(t.key)}
          className={
            "rounded-md px-3 py-1.5 " +
            (current === t.key
              ? "bg-neutral-900 text-white"
              : "text-neutral-700 hover:bg-neutral-100")
          }
        >
          {t.label}
        </Link>
      ))}
    </div>
  );
}
