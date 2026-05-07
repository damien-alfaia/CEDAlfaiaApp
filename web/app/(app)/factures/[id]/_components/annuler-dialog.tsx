"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { X } from "@/components/icons";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

type Props = {
  action: (commentaire: string) => Promise<void>;
};

export function AnnulerFactureDialog({ action }: Props) {
  const [open, setOpen] = useState(false);
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [commentaire, setCommentaire] = useState("");

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm" className="text-red-600 hover:text-red-700">
          <X className="mr-1 h-4 w-4" />
          Annuler la facture
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Annuler la facture ?</DialogTitle>
          <DialogDescription>
            Indiquez le motif. Vous pourrez la repasser en facture si besoin.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-2">
          <Label htmlFor="commentaire">Motif</Label>
          <Textarea
            id="commentaire"
            value={commentaire}
            onChange={(e) => setCommentaire(e.target.value)}
            rows={3}
            placeholder="ex: erreur de saisie"
          />
        </div>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <DialogFooter>
          <Button variant="ghost" onClick={() => setOpen(false)} disabled={pending}>
            Retour
          </Button>
          <Button
            variant="destructive"
            disabled={pending}
            onClick={async () => {
              setPending(true);
              setError(null);
              try {
                await action(commentaire);
                setOpen(false);
              } catch (e) {
                setError(e instanceof Error ? e.message : "Erreur");
              } finally {
                setPending(false);
              }
            }}
          >
            {pending ? "Annulation…" : "Annuler la facture"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
