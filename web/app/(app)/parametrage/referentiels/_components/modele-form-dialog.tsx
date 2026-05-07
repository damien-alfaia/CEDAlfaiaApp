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
import type { ActionState } from "../actions";

type Props = {
  trigger: React.ReactNode;
  title: string;
  marqueId: number;
  defaultValues?: { libelle: string };
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

const initial: ActionState = {};

export function ModeleFormDialog({ trigger, title, marqueId, defaultValues, action }: Props) {
  const [open, setOpen] = useState(false);
  const [state, formAction, pending] = useActionState(action, initial);

  useEffect(() => {
    // Closing the dialog upon a successful Server Action is intentional
    // and one of the few legitimate reasons to setState in an effect.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    if (state.ok) setOpen(false);
  }, [state.ok]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent className="sm:max-w-sm">
        <form action={formAction} className="space-y-4">
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
          </DialogHeader>

          <input type="hidden" name="marque_id" value={marqueId} />

          <div className="space-y-2">
            <Label htmlFor="libelle">
              Libellé <span className="text-red-600">*</span>
            </Label>
            <Input
              id="libelle"
              name="libelle"
              defaultValue={defaultValues?.libelle ?? ""}
              required
            />
            {state.fieldErrors?.libelle && (
              <p className="text-xs text-red-600">{state.fieldErrors.libelle}</p>
            )}
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
