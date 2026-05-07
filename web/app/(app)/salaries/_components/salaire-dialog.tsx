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

const initial: ActionState = {};

type Props = {
  trigger: React.ReactNode;
  contratId: number;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

export function SalaireDialog({ trigger, contratId, action }: Props) {
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
            <DialogTitle>Nouveau bulletin de salaire</DialogTitle>
          </DialogHeader>

          <input type="hidden" name="salarie_contrat_id" value={contratId} />

          <div className="grid gap-3 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="date_debut">
                Période — début <span className="text-red-600">*</span>
              </Label>
              <Input id="date_debut" name="date_debut" type="date" required />
            </div>
            <div className="space-y-2">
              <Label htmlFor="date_fin">
                Période — fin <span className="text-red-600">*</span>
              </Label>
              <Input id="date_fin" name="date_fin" type="date" required />
            </div>
          </div>

          <div className="grid gap-3 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="date_paiement">
                Date de paiement <span className="text-red-600">*</span>
              </Label>
              <Input id="date_paiement" name="date_paiement" type="date" required />
            </div>
            <div className="space-y-2">
              <Label htmlFor="salaire_net">
                Salaire net (€) <span className="text-red-600">*</span>
              </Label>
              <Input
                id="salaire_net"
                name="salaire_net"
                type="number"
                step="0.01"
                min={0}
                required
              />
            </div>
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
