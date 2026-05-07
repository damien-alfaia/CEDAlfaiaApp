"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Trash2 } from "@/components/icons";
import { softDeleteVoitureAction } from "../../actions";

type Props = {
  voitureId: number;
  clientId: number;
  label: string;
};

export function VoitureDeleteButton({ voitureId, clientId, label }: Props) {
  const [open, setOpen] = useState(false);
  const [pending, setPending] = useState(false);
  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="ghost" size="icon" aria-label="Supprimer">
          <Trash2 className="h-4 w-4 text-red-600" />
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>Supprimer cette voiture ?</DialogTitle>
          <DialogDescription>
            <span className="font-medium">{label}</span> sera retirée de la fiche client. La
            suppression est logique : les devis/factures historiques restent.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="ghost" onClick={() => setOpen(false)} disabled={pending}>
            Annuler
          </Button>
          <Button
            variant="destructive"
            disabled={pending}
            onClick={async () => {
              setPending(true);
              try {
                await softDeleteVoitureAction(voitureId, clientId);
                setOpen(false);
              } finally {
                setPending(false);
              }
            }}
          >
            {pending ? "Suppression…" : "Supprimer"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
