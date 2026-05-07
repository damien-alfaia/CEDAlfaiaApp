"use client";

import { useState, useActionState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
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
import type { ActionState } from "../../actions";
import type { VoitureWithModele } from "@/lib/db/types";

type ModeleOption = { id: number; label: string; marqueLibelle: string };

type Props = {
  trigger: React.ReactNode;
  title: string;
  clientId: number;
  voiture?: VoitureWithModele;
  modeles: ModeleOption[];
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

const initial: ActionState = {};

export function VoitureDialog({ trigger, title, clientId, voiture, modeles, action }: Props) {
  const [open, setOpen] = useState(false);
  const [state, formAction, pending] = useActionState(action, initial);
  const [modeleId, setModeleId] = useState<string>(voiture?.modele_id?.toString() ?? "");

  // Close on success — legitimate setState-in-effect pattern with Server Actions.
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

          <input type="hidden" name="client_id" value={clientId} />

          <div className="space-y-2">
            <Label htmlFor="modele_id">
              Modèle <span className="text-red-600">*</span>
            </Label>
            <Select name="modele_id" value={modeleId} onValueChange={setModeleId}>
              <SelectTrigger id="modele_id">
                <SelectValue placeholder="Choisir un modèle…" />
              </SelectTrigger>
              <SelectContent>
                {modeles.length === 0 && (
                  <div className="px-2 py-3 text-sm text-neutral-500">
                    Aucun modèle. Ajoutez-en dans Paramétrage → Référentiels.
                  </div>
                )}
                {modeles.map((m) => (
                  <SelectItem key={m.id} value={m.id.toString()}>
                    {m.marqueLibelle} — {m.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {state.fieldErrors?.modele_id && (
              <p className="text-xs text-red-600">{state.fieldErrors.modele_id}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="immatriculation">Immatriculation</Label>
            <Input
              id="immatriculation"
              name="immatriculation"
              defaultValue={voiture?.immatriculation ?? ""}
              placeholder="ex: AB-123-CD"
            />
          </div>

          <div className="flex items-center gap-2">
            <Checkbox
              id="is_principale"
              name="is_principale"
              defaultChecked={voiture?.is_principale ?? false}
            />
            <Label htmlFor="is_principale" className="font-normal">
              Voiture principale
            </Label>
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
