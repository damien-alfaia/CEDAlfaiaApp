import Link from "next/link";
import { Plus } from "@/components/icons";
import { requireRole } from "@/lib/auth";
import { listSalaries } from "@/lib/data/salaries";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export default async function SalariesPage() {
  await requireRole(["super_admin", "administrateur"]);
  const rows = await listSalaries();

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Salariés</h1>
          <p className="text-sm text-neutral-500">
            {rows.length} {rows.length > 1 ? "résultats" : "résultat"}
          </p>
        </div>
        <Button asChild>
          <Link href="/salaries/new">
            <Plus className="mr-2 h-4 w-4" />
            Nouveau salarié
          </Link>
        </Button>
      </div>

      <Card>
        <CardContent className="p-4">
          {rows.length === 0 ? (
            <div className="rounded-md border border-dashed p-8 text-center text-sm text-neutral-500">
              Aucun salarié à afficher.
            </div>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Nom</TableHead>
                    <TableHead className="hidden sm:table-cell">Téléphone</TableHead>
                    <TableHead className="hidden md:table-cell">Email</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {rows.map((s) => (
                    <TableRow key={s.id}>
                      <TableCell className="font-medium">
                        <Link href={`/salaries/${s.id}`} className="hover:underline">
                          {[s.prenom, s.nom].filter(Boolean).join(" ") || "(sans nom)"}
                        </Link>
                      </TableCell>
                      <TableCell className="hidden sm:table-cell">
                        {s.portable ?? s.telephone ?? "—"}
                      </TableCell>
                      <TableCell className="hidden md:table-cell">{s.email ?? "—"}</TableCell>
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
