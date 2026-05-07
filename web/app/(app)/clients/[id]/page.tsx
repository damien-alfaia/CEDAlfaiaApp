import Link from "next/link";
import { notFound } from "next/navigation";
import { ArrowLeft, Plus, Pencil, Star } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ClientForm } from "../_components/client-form";
import { createVoitureAction, updateClientAction, updateVoitureAction } from "../actions";
import { ClientDeleteButtons } from "./_components/client-delete-buttons";
import { VoitureDialog } from "./_components/voiture-dialog";
import { VoitureDeleteButton } from "./_components/voiture-delete-button";
import { getClient } from "@/lib/data/clients";
import { listVoituresByClient } from "@/lib/data/voitures";
import { listAllModelesGrouped } from "@/lib/data/referentiels";

export default async function ClientDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const clientId = Number(id);
  if (!Number.isInteger(clientId) || clientId <= 0) notFound();

  const client = await getClient(clientId);
  if (!client) notFound();

  const [voitures, modeles] = await Promise.all([
    listVoituresByClient(clientId),
    listAllModelesGrouped(),
  ]);

  const updateBound = updateClientAction.bind(null, clientId);
  const createVoiture = createVoitureAction;

  const fullName = [client.prenom, client.nom].filter(Boolean).join(" ").trim() || client.nom;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-3">
        <Button asChild variant="ghost" size="sm">
          <Link href="/clients">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour aux clients
          </Link>
        </Button>
        <ClientDeleteButtons
          clientId={clientId}
          isDeleted={client.date_suppression !== null}
          clientName={fullName}
        />
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <h1 className="text-2xl font-bold tracking-tight">{fullName}</h1>
        {client.date_suppression ? (
          <Badge variant="secondary">Supprimé</Badge>
        ) : client.is_prospect ? (
          <Badge variant="outline">Prospect</Badge>
        ) : (
          <Badge>Client</Badge>
        )}
      </div>

      <ClientForm client={client} action={updateBound} submitLabel="Enregistrer" />

      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0">
          <CardTitle className="text-base">Voitures</CardTitle>
          <VoitureDialog
            trigger={
              <Button size="sm">
                <Plus className="mr-1 h-4 w-4" />
                Ajouter une voiture
              </Button>
            }
            title="Ajouter une voiture"
            clientId={clientId}
            modeles={modeles}
            action={createVoiture}
          />
        </CardHeader>
        <CardContent>
          {voitures.length === 0 ? (
            <div className="rounded-md border border-dashed p-8 text-center text-sm text-muted-foreground">
              Aucune voiture enregistrée pour ce client.
            </div>
          ) : (
            <div className="overflow-x-auto rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Marque</TableHead>
                    <TableHead>Modèle</TableHead>
                    <TableHead>Immatriculation</TableHead>
                    <TableHead>Principale</TableHead>
                    <TableHead className="w-[120px] text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {voitures.map((v) => {
                    const updateVoitureBound = updateVoitureAction.bind(null, v.id);
                    const label = `${v.modele?.marque?.libelle ?? "—"} ${v.modele?.libelle ?? ""}${v.immatriculation ? ` (${v.immatriculation})` : ""}`;
                    return (
                      <TableRow key={v.id}>
                        <TableCell>{v.modele?.marque?.libelle ?? "—"}</TableCell>
                        <TableCell>{v.modele?.libelle ?? "—"}</TableCell>
                        <TableCell className="font-mono">{v.immatriculation ?? "—"}</TableCell>
                        <TableCell>
                          {v.is_principale && (
                            <Star className="h-4 w-4 fill-yellow-500 text-yellow-500" />
                          )}
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex justify-end gap-1">
                            <VoitureDialog
                              trigger={
                                <Button variant="ghost" size="icon" aria-label="Éditer">
                                  <Pencil className="h-4 w-4" />
                                </Button>
                              }
                              title="Modifier la voiture"
                              clientId={clientId}
                              voiture={v}
                              modeles={modeles}
                              action={updateVoitureBound}
                            />
                            <VoitureDeleteButton
                              voitureId={v.id}
                              clientId={clientId}
                              label={label}
                            />
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
