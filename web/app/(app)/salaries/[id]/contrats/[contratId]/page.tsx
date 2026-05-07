import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, Plus, Trash2 } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { requireRole } from "@/lib/auth";
import { getSalarie } from "@/lib/data/salaries";
import {
  getContrat,
  listIndisponibilitesForContrat,
  listSalairesForContrat,
} from "@/lib/data/salaries-sub";
import { fmtDate, fmtEuro } from "@/lib/format";
import { IndispoDialog } from "../../../_components/indispo-dialog";
import { SalaireDialog } from "../../../_components/salaire-dialog";
import { ConfirmActionButton } from "../../../../_components/confirm-action-button";
import {
  createIndisponibiliteAction,
  createSalaireAction,
  deleteIndisponibiliteAction,
  deleteSalaireAction,
} from "../../../_actions/sub";

const TYPE_INDISPO_LABEL: Record<string, string> = {
  absence: "Absence",
  conges: "Congés",
  ecole: "École",
  autre: "Autre",
};

export default async function ContratDetailPage({
  params,
}: {
  params: Promise<{ id: string; contratId: string }>;
}) {
  await requireRole(["super_admin", "administrateur"]);
  const { id, contratId: rawContratId } = await params;
  const sId = Number(id);
  const cId = Number(rawContratId);
  if (!Number.isInteger(sId) || sId <= 0) notFound();
  if (!Number.isInteger(cId) || cId <= 0) notFound();

  const [salarie, contrat, indispos, salaires] = await Promise.all([
    getSalarie(sId),
    getContrat(cId),
    listIndisponibilitesForContrat(cId),
    listSalairesForContrat(cId),
  ]);
  if (!salarie || !contrat || contrat.salarie_id !== sId) notFound();

  const fullName = [salarie.prenom, salarie.nom].filter(Boolean).join(" ").trim() || "(sans nom)";
  const enCours = !contrat.date_fin || new Date(contrat.date_fin) > new Date();
  const totalSalaires = salaires.reduce((a, s) => a + (s.salaire_net ?? 0), 0);

  return (
    <div className="space-y-6">
      <div>
        <Button asChild variant="ghost" size="sm">
          <Link href={`/salaries/${sId}`}>
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour à {fullName}
          </Link>
        </Button>
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <h1 className="text-2xl font-bold tracking-tight">{contrat.type_contrat ?? "Contrat"}</h1>
        {enCours ? <Badge>En cours</Badge> : <Badge variant="secondary">Terminé</Badge>}
      </div>
      <p className="text-sm text-muted-foreground">
        Du {fmtDate(contrat.date_debut)} au {contrat.date_fin ? fmtDate(contrat.date_fin) : "—"}
      </p>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0">
          <CardTitle className="text-base">Indisponibilités ({indispos.length})</CardTitle>
          <IndispoDialog
            trigger={
              <Button size="sm">
                <Plus className="mr-1 h-4 w-4" />
                Ajouter
              </Button>
            }
            contratId={cId}
            action={createIndisponibiliteAction}
          />
        </CardHeader>
        <CardContent>
          {indispos.length === 0 ? (
            <p className="text-sm text-muted-foreground">Aucune indisponibilité.</p>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Type</TableHead>
                    <TableHead>Début</TableHead>
                    <TableHead>Fin</TableHead>
                    <TableHead>Motif</TableHead>
                    <TableHead className="w-12 text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {indispos.map((i) => (
                    <TableRow key={i.id}>
                      <TableCell>{TYPE_INDISPO_LABEL[i.type_indisponibilite]}</TableCell>
                      <TableCell>{fmtDate(i.date_debut)}</TableCell>
                      <TableCell>{fmtDate(i.date_fin)}</TableCell>
                      <TableCell className="text-muted-foreground">{i.motif ?? "—"}</TableCell>
                      <TableCell className="text-right">
                        <ConfirmActionButton
                          label=""
                          confirmTitle="Supprimer cette indisponibilité ?"
                          confirmDescription="Action définitive."
                          confirmCta="Supprimer"
                          variant="ghost"
                          destructive
                          icon={<Trash2 className="h-4 w-4" />}
                          action={deleteIndisponibiliteAction.bind(null, i.id)}
                        />
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0">
          <CardTitle className="text-base">
            Bulletins de salaire ({salaires.length}) — total versé{" "}
            <span className="font-semibold">{fmtEuro(totalSalaires)}</span>
          </CardTitle>
          <SalaireDialog
            trigger={
              <Button size="sm">
                <Plus className="mr-1 h-4 w-4" />
                Ajouter
              </Button>
            }
            contratId={cId}
            action={createSalaireAction}
          />
        </CardHeader>
        <CardContent>
          {salaires.length === 0 ? (
            <p className="text-sm text-muted-foreground">Aucun bulletin.</p>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Période</TableHead>
                    <TableHead>Date paiement</TableHead>
                    <TableHead className="text-right">Net</TableHead>
                    <TableHead className="w-12 text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {salaires.map((s) => (
                    <TableRow key={s.id}>
                      <TableCell>
                        {fmtDate(s.date_debut)} → {fmtDate(s.date_fin)}
                      </TableCell>
                      <TableCell>{fmtDate(s.date_paiement)}</TableCell>
                      <TableCell className="text-right tabular-nums">
                        {fmtEuro(s.salaire_net)}
                      </TableCell>
                      <TableCell className="text-right">
                        <ConfirmActionButton
                          label=""
                          confirmTitle="Supprimer ce bulletin ?"
                          confirmDescription="Action définitive."
                          confirmCta="Supprimer"
                          variant="ghost"
                          destructive
                          icon={<Trash2 className="h-4 w-4" />}
                          action={deleteSalaireAction.bind(null, s.id)}
                        />
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
