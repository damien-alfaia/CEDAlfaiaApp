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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { ActionState } from "../_actions/sub";

const initial: ActionState = {};

type Props = {
  trigger: React.ReactNode;
  contratId: number;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

export function IndispoDialog({ trigger, contratId, action }: Props) {
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
            <DialogTitle>Nouvelle indisponibilité</DialogTitle>
          </DialogHeader>

          <input type="hidden" name="salarie_contrat_id" value={contratId} />

          <div className="space-y-2">
            <Label htmlFor="type_indisponibilite">Type</Label>
            <Select name="type_indisponibilite" defaultValue="conges">
              <SelectTrigger id="type_indisponibilite">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="conges">Congés</SelectItem>
                <SelectItem value="absence">Absence</SelectItem>
                <SelectItem value="ecole">École</SelectItem>
                <SelectItem value="autre">Autre</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="grid gap-3 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="date_debut">
                Début <span className="text-red-600">*</span>
              </Label>
              <Input id="date_debut" name="date_debut" type="datetime-local" required />
            </div>
            <div className="space-y-2">
              <Label htmlFor="date_fin">
                Fin <span className="text-red-600">*</span>
              </Label>
              <Input id="date_fin" name="date_fin" type="datetime-local" required />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="motif">Motif</Label>
            <Textarea id="motif" name="motif" rows={2} />
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
