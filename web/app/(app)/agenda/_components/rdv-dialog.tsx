"use client";

import { useEffect, useState, useActionState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Alert, AlertDescription } from "@/components/ui/alert";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import type { ActionState } from "../actions";
import type { RendezVousRow } from "@/lib/db/types";

const initial: ActionState = {};

type Props = {
  trigger: React.ReactNode;
  title: string;
  rdv?: RendezVousRow;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

function toLocalDateTime(iso: string | null | undefined): string {
  if (!iso) return "";
  // input type="datetime-local" attend YYYY-MM-DDTHH:MM
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "";
  const pad = (n: number) => n.toString().padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function RdvDialog({ trigger, title, rdv, action }: Props) {
  const [open, setOpen] = useState(false);
  const [state, formAction, pending] = useActionState(action, initial);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    if (state.ok) setOpen(false);
  }, [state.ok]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <form action={formAction} className="space-y-4">
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
          </DialogHeader>

          <div className="space-y-2">
            <Label htmlFor="sujet">
              Sujet <span className="text-red-600">*</span>
            </Label>
            <Input id="sujet" name="sujet" defaultValue={rdv?.sujet ?? ""} required />
            {state.fieldErrors?.sujet && (
              <p className="text-xs text-red-600">{state.fieldErrors.sujet}</p>
            )}
          </div>

          <div className="grid gap-3 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="date_heure_debut">
                Début <span className="text-red-600">*</span>
              </Label>
              <Input
                id="date_heure_debut"
                name="date_heure_debut"
                type="datetime-local"
                defaultValue={toLocalDateTime(rdv?.date_heure_debut)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="date_heure_fin">Fin</Label>
              <Input
                id="date_heure_fin"
                name="date_heure_fin"
                type="datetime-local"
                defaultValue={toLocalDateTime(rdv?.date_heure_fin)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="duree">Durée (minutes)</Label>
              <Input
                id="duree"
                name="duree"
                type="number"
                min={0}
                defaultValue={rdv?.duree?.toString() ?? ""}
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="commentaire">Commentaire</Label>
            <Textarea
              id="commentaire"
              name="commentaire"
              defaultValue={rdv?.commentaire ?? ""}
              rows={3}
            />
          </div>

          {state.error && (
            <Alert variant="destructive">
              <AlertDescription>{state.error}</AlertDescription>
            </Alert>
          )}

          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => setOpen(false)}>
              Annuler
            </Button>
            <Button type="submit" disabled={pending}>
              {pending ? "Enregistrement…" : "Enregistrer"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
