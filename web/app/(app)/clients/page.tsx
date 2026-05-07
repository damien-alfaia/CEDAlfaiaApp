import Link from "next/link";
import { Plus, Search } from "@/components/icons";
import { listClients, type ClientFilter, type ClientProspectFilter } from "@/lib/data/clients";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
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
import { Pagination } from "./_components/pagination";
import { FilterTabs } from "./_components/filter-tabs";

type SearchParams = Promise<{
  q?: string;
  filter?: string;
  prospect?: string;
  page?: string;
}>;

export default async function ClientsPage({ searchParams }: { searchParams: SearchParams }) {
  const sp = await searchParams;
  const q = sp.q?.toString() ?? "";
  const filter = (sp.filter as ClientFilter) ?? "actif";
  const prospect = (sp.prospect as ClientProspectFilter) ?? "tous";
  const page = Math.max(1, Number(sp.page ?? 1) || 1);

  const result = await listClients({ q, filter, prospect, page });

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Clients</h1>
          <p className="text-sm text-muted-foreground">
            {result.total} {result.total > 1 ? "résultats" : "résultat"}
          </p>
        </div>
        <Button asChild>
          <Link href="/clients/new">
            <Plus className="mr-2 h-4 w-4" />
            Nouveau client
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
                placeholder="Rechercher (nom, prénom, email, téléphone, code)"
                className="pl-9"
              />
            </div>
            <input type="hidden" name="filter" value={filter} />
            <input type="hidden" name="prospect" value={prospect} />
            <Button type="submit" variant="secondary">
              Rechercher
            </Button>
          </form>

          <div className="flex flex-wrap items-center justify-between gap-3">
            <FilterTabs current={filter} prospect={prospect} q={q} />
          </div>

          <ClientsTable rows={result.rows} />

          <Pagination
            page={result.page}
            pageCount={result.pageCount}
            q={q}
            filter={filter}
            prospect={prospect}
          />
        </CardContent>
      </Card>
    </div>
  );
}

function ClientsTable({ rows }: { rows: Awaited<ReturnType<typeof listClients>>["rows"] }) {
  if (rows.length === 0) {
    return (
      <div className="rounded-md border border-dashed p-8 text-center text-sm text-muted-foreground">
        Aucun client à afficher.
      </div>
    );
  }
  return (
    <div className="overflow-x-auto rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Nom</TableHead>
            <TableHead className="hidden sm:table-cell">Téléphone</TableHead>
            <TableHead className="hidden md:table-cell">Email</TableHead>
            <TableHead className="hidden lg:table-cell">Ville</TableHead>
            <TableHead>Statut</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {rows.map((c) => {
            const fullName = [c.prenom, c.nom].filter(Boolean).join(" ");
            return (
              <TableRow key={c.id} className="cursor-pointer">
                <TableCell className="font-medium">
                  <Link href={`/clients/${c.id}`} className="hover:underline">
                    {fullName || c.nom}
                  </Link>
                  {c.code && <span className="ml-2 text-xs text-muted-foreground">{c.code}</span>}
                </TableCell>
                <TableCell className="hidden sm:table-cell">{c.telephone ?? "—"}</TableCell>
                <TableCell className="hidden md:table-cell">{c.email ?? "—"}</TableCell>
                <TableCell className="hidden lg:table-cell">{c.adresse_ville ?? "—"}</TableCell>
                <TableCell>
                  {c.date_suppression ? (
                    <Badge variant="secondary">Supprimé</Badge>
                  ) : c.is_prospect ? (
                    <Badge variant="outline">Prospect</Badge>
                  ) : (
                    <Badge>Client</Badge>
                  )}
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </div>
  );
}
