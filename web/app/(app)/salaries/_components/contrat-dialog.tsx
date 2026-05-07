"use client";

import { useEffect, useState, useActionState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import type { ActionState } from "../_actions/sub";
import type { SalarieContratRow } from "@/lib/db/types";

const initial: ActionState = {};

type Props = {
  trigger: React.ReactNode;
  title: string;
  salarieId: number;
  contrat?: SalarieContratRow;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

export function ContratDialog({ trigger, title, salarieId, contrat, action }: Props) {
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

          <input type="hidden" name="salarie_id" value={salarieId} />

          <div className="grid gap-3 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="date_debut">
                Début <span className="text-red-600">*</span>
              </Label>
              <Input
                id="date_debut"
                name="date_debut"
                type="date"
                defaultValue={contrat?.date_debut?.slice(0, 10) ?? ""}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="date_fin">Fin (vide = en cours)</Label>
              <Input
                id="date_fin"
                name="date_fin"
                type="date"
                defaultValue={contrat?.date_fin?.slice(0, 10) ?? ""}
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="type_contrat">Type de contrat</Label>
            <Input
              id="type_contrat"
              name="type_contrat"
              defaultValue={contrat?.type_contrat ?? ""}
              placeholder="ex: CDI, CDD, apprentissage…"
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
