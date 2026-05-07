import Link from "next/link";
import { Plus, Search } from "@/components/icons";
import { listFournisseurs } from "@/lib/data/fournisseurs";
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
import { Badge } from "@/components/ui/badge";

type SearchParams = Promise<{ q?: string; deleted?: string }>;

export default async function FournisseursPage({ searchParams }: { searchParams: SearchParams }) {
  const sp = await searchParams;
  const q = sp.q?.toString() ?? "";
  const showDeleted = sp.deleted === "1";
  const rows = await listFournisseurs({ q, showDeleted });

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Fournisseurs</h1>
          <p className="text-sm text-muted-foreground">
            {rows.length} {rows.length > 1 ? "résultats" : "résultat"}
          </p>
        </div>
        <Button asChild>
          <Link href="/fournisseurs/new">
            <Plus className="mr-2 h-4 w-4" />
            Nouveau fournisseur
          </Link>
        </Button>
      </div>

      <Card>
        <CardContent className="space-y-4 p-4">
          <form className="flex flex-wrap items-center gap-2">
            <div className="relative flex-1 min-w-[200px]">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input name="q" defaultValue={q} placeholder="Rechercher par nom" className="pl-9" />
            </div>
            {showDeleted && <input type="hidden" name="deleted" value="1" />}
            <Button type="submit" variant="secondary">
              Rechercher
            </Button>
          </form>

          <div className="flex items-center gap-1 rounded-md border bg-card p-1 text-sm">
            <span className="px-2 text-xs uppercase tracking-wider text-muted-foreground">
              Statut
            </span>
            <Link
              href={q ? `/fournisseurs?q=${encodeURIComponent(q)}` : "/fournisseurs"}
              className={
                "rounded-md px-3 py-1.5 " +
                (!showDeleted
                  ? "bg-primary text-primary-foreground"
                  : "text-foreground hover:bg-muted")
              }
            >
              Actifs
            </Link>
            <Link
              href={
                q ? `/fournisseurs?q=${encodeURIComponent(q)}&deleted=1` : "/fournisseurs?deleted=1"
              }
              className={
                "rounded-md px-3 py-1.5 " +
                (showDeleted
                  ? "bg-primary text-primary-foreground"
                  : "text-foreground hover:bg-muted")
              }
            >
              Supprimés
            </Link>
          </div>

          {rows.length === 0 ? (
            <div className="rounded-md border border-dashed p-8 text-center text-sm text-muted-foreground">
              Aucun fournisseur à afficher.
            </div>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Nom</TableHead>
                    <TableHead className="hidden md:table-cell">Ville</TableHead>
                    <TableHead className="hidden lg:table-cell">Code postal</TableHead>
                    <TableHead>Statut</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {rows.map((f) => (
                    <TableRow key={f.id}>
                      <TableCell className="font-medium">
                        <Link href={`/fournisseurs/${f.id}`} className="hover:underline">
                          {f.nom}
                        </Link>
                      </TableCell>
                      <TableCell className="hidden md:table-cell">
                        {f.adresse_ville ?? "—"}
                      </TableCell>
                      <TableCell className="hidden lg:table-cell">
                        {f.adresse_code_postal ?? "—"}
                      </TableCell>
                      <TableCell>
                        {f.date_suppression ? (
                          <Badge variant="secondary">Supprimé</Badge>
                        ) : (
                          <Badge>Actif</Badge>
                        )}
                      </TableCell>
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
