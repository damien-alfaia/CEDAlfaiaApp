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
import { ArchiveRestore, Trash2 } from "@/components/icons";
import { restoreClientAction, softDeleteClientAction } from "../../actions";

type Props = {
  clientId: number;
  isDeleted: boolean;
  clientName: string;
};

export function ClientDeleteButtons({ clientId, isDeleted, clientName }: Props) {
  const [open, setOpen] = useState(false);
  const [pending, setPending] = useState(false);

  if (isDeleted) {
    return (
      <Button
        variant="outline"
        disabled={pending}
        onClick={async () => {
          setPending(true);
          try {
            await restoreClientAction(clientId);
          } finally {
            setPending(false);
          }
        }}
      >
        <ArchiveRestore className="mr-2 h-4 w-4" />
        Restaurer
      </Button>
    );
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline" className="text-red-600 hover:text-red-700">
          <Trash2 className="mr-2 h-4 w-4" />
          Supprimer
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>Supprimer le client ?</DialogTitle>
          <DialogDescription>
            <span className="font-medium">{clientName}</span> sera marqué comme supprimé
            (suppression logique). Les devis et factures associés restent consultables. Vous pouvez
            restaurer le client à tout moment.
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
                await softDeleteClientAction(clientId);
              } finally {
                setPending(false);
              }
            }}
          >
            {pending ? "Suppression…" : "Confirmer la suppression"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
