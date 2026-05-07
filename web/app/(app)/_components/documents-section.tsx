"use client";

import { useActionState, useEffect, useRef } from "react";
import { FileDown, Plus, Trash2 } from "@/components/icons";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { uploadDocumentAction, type UploadState } from "../_actions/documents";
import { ConfirmActionButton } from "./confirm-action-button";
import type { TypeDocument } from "@/lib/db/types";
import { fmtDate } from "@/lib/format";

type DocumentItem = {
  id: number;
  libelle: string | null;
  type_document: TypeDocument;
  storage_path: string | null;
  format_fichier: string | null;
  date_creation: string | null;
  /** URL signée (générée côté serveur, valide quelques minutes). */
  signedUrl: string | null;
};

type Props = {
  pieceVenteId: number;
  variant: "devis" | "facture";
  documents: DocumentItem[];
  onDelete: (documentId: number) => Promise<void>;
};

const initial: UploadState = {};

const TYPE_LABEL: Record<TypeDocument, string> = {
  devis_fournisseur: "Devis fournisseur",
  bon_livraison_fournisseur: "Bon de livraison fournisseur",
  devis: "Devis",
  facture: "Facture",
};

export function DocumentsSection({ pieceVenteId, variant, documents, onDelete }: Props) {
  const [state, formAction, pending] = useActionState(uploadDocumentAction, initial);
  const formRef = useRef<HTMLFormElement>(null);

  // Reset le form après un upload réussi.
  useEffect(() => {
    if (state.ok && formRef.current) {
      formRef.current.reset();
    }
  }, [state.ok]);

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">Documents joints</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {documents.length === 0 ? (
          <p className="text-sm text-muted-foreground">Aucun document joint.</p>
        ) : (
          <ul className="divide-y divide-border rounded-md border">
            {documents.map((d) => (
              <li key={d.id} className="flex items-center justify-between gap-3 px-3 py-2 text-sm">
                <div className="min-w-0 flex-1">
                  <p className="truncate font-medium">{d.libelle ?? "(sans nom)"}</p>
                  <p className="text-xs text-muted-foreground">
                    {TYPE_LABEL[d.type_document]} · {fmtDate(d.date_creation)}
                  </p>
                </div>
                <div className="flex items-center gap-1">
                  {d.signedUrl ? (
                    <Button asChild variant="ghost" size="icon" aria-label="Télécharger">
                      <a href={d.signedUrl} target="_blank" rel="noopener noreferrer">
                        <FileDown className="h-4 w-4" />
                      </a>
                    </Button>
                  ) : (
                    <Button variant="ghost" size="icon" disabled aria-label="Indisponible">
                      <FileDown className="h-4 w-4 opacity-30" />
                    </Button>
                  )}
                  <ConfirmActionButton
                    label=""
                    confirmTitle="Supprimer ce document ?"
                    confirmDescription={`"${d.libelle ?? "(sans nom)"}" sera supprimé du Storage et de la base.`}
                    confirmCta="Supprimer"
                    variant="ghost"
                    destructive
                    icon={<Trash2 className="h-4 w-4" />}
                    action={onDelete.bind(null, d.id)}
                  />
                </div>
              </li>
            ))}
          </ul>
        )}

        <form ref={formRef} action={formAction} className="grid gap-3 md:grid-cols-3">
          <input type="hidden" name="piece_vente_id" value={pieceVenteId} />

          <div className="space-y-2">
            <Label htmlFor="type_document">Type</Label>
            <Select name="type_document" defaultValue="devis_fournisseur">
              <SelectTrigger id="type_document">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="devis_fournisseur">Devis fournisseur</SelectItem>
                <SelectItem value="bon_livraison_fournisseur">
                  Bon de livraison fournisseur
                </SelectItem>
                {variant === "devis" && <SelectItem value="devis">Devis (autre)</SelectItem>}
                {variant === "facture" && <SelectItem value="facture">Facture (autre)</SelectItem>}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="file">Fichier (PDF ou image, max 10 MiB)</Label>
            <Input
              id="file"
              name="file"
              type="file"
              accept="application/pdf,image/png,image/jpeg,image/jpg,image/webp,image/heic"
              required
            />
          </div>

          {state.error && (
            <div className="md:col-span-3">
              <Alert variant="destructive">
                <AlertDescription>{state.error}</AlertDescription>
              </Alert>
            </div>
          )}
          {state.ok && (
            <div className="md:col-span-3">
              <Alert>
                <AlertDescription>Document ajouté.</AlertDescription>
              </Alert>
            </div>
          )}

          <div className="md:col-span-3 flex justify-end">
            <Button type="submit" disabled={pending}>
              <Plus className="mr-1 h-4 w-4" />
              {pending ? "Upload…" : "Ajouter le document"}
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
