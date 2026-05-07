import { Pencil, Plus } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { requireRole } from "@/lib/auth";
import { listMarquesWithModeles } from "@/lib/data/referentiels";
import {
  createMarqueAction,
  createModeleAction,
  deleteMarqueAction,
  deleteModeleAction,
  updateMarqueAction,
  updateModeleAction,
} from "./actions";
import { MarqueFormDialog } from "./_components/marque-form-dialog";
import { ModeleFormDialog } from "./_components/modele-form-dialog";
import { DeleteButton } from "./_components/delete-button";

export default async function ReferentielsPage() {
  await requireRole(["super_admin", "administrateur"]);
  const marques = await listMarquesWithModeles();

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Référentiels — Marques & modèles</h1>
          <p className="text-sm text-muted-foreground">
            Catalogue utilisé pour rattacher les voitures clients à un modèle.
          </p>
        </div>
        <MarqueFormDialog
          trigger={
            <Button>
              <Plus className="mr-1 h-4 w-4" />
              Nouvelle marque
            </Button>
          }
          title="Nouvelle marque"
          action={createMarqueAction}
        />
      </div>

      {marques.length === 0 ? (
        <Card>
          <CardContent className="p-8 text-center text-sm text-muted-foreground">
            Aucune marque — commencez par en ajouter une.
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4">
          {marques.map((m) => {
            const updateMarqueBound = updateMarqueAction.bind(null, m.id);
            return (
              <Card key={m.id}>
                <CardHeader className="flex flex-row items-center justify-between space-y-0">
                  <CardTitle className="flex items-center gap-3 text-base">
                    <span className="rounded bg-muted px-2 py-0.5 font-mono text-xs">{m.code}</span>
                    {m.libelle}
                  </CardTitle>
                  <div className="flex items-center gap-1">
                    <MarqueFormDialog
                      trigger={
                        <Button variant="ghost" size="icon" aria-label="Éditer">
                          <Pencil className="h-4 w-4" />
                        </Button>
                      }
                      title="Modifier la marque"
                      defaultValues={{ code: m.code, libelle: m.libelle }}
                      action={updateMarqueBound}
                    />
                    <DeleteButton
                      label={m.libelle}
                      onConfirm={deleteMarqueAction.bind(null, m.id)}
                    />
                  </div>
                </CardHeader>
                <CardContent className="space-y-3">
                  <Separator />
                  <div className="space-y-1">
                    {m.modeles.length === 0 ? (
                      <p className="text-sm text-muted-foreground">Aucun modèle.</p>
                    ) : (
                      m.modeles.map((mo) => {
                        const updateModeleBound = updateModeleAction.bind(null, mo.id);
                        return (
                          <div
                            key={mo.id}
                            className="flex items-center justify-between rounded-md py-1 hover:bg-muted/40"
                          >
                            <span className="text-sm">{mo.libelle}</span>
                            <div className="flex items-center">
                              <ModeleFormDialog
                                trigger={
                                  <Button variant="ghost" size="icon" aria-label="Éditer">
                                    <Pencil className="h-4 w-4" />
                                  </Button>
                                }
                                title="Modifier le modèle"
                                marqueId={m.id}
                                defaultValues={{ libelle: mo.libelle }}
                                action={updateModeleBound}
                              />
                              <DeleteButton
                                label={mo.libelle}
                                onConfirm={deleteModeleAction.bind(null, mo.id)}
                              />
                            </div>
                          </div>
                        );
                      })
                    )}
                  </div>
                  <ModeleFormDialog
                    trigger={
                      <Button variant="outline" size="sm">
                        <Plus className="mr-1 h-4 w-4" />
                        Ajouter un modèle
                      </Button>
                    }
                    title="Nouveau modèle"
                    marqueId={m.id}
                    action={createModeleAction}
                  />
                </CardContent>
              </Card>
            );
          })}
        </div>
      )}
    </div>
  );
}
