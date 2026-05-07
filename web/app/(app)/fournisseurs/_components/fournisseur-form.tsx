"use client";

import { useActionState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { FournisseurRow } from "@/lib/db/types";
import type { ActionState } from "../actions";

const initial: ActionState = {};

type Props = {
  fournisseur?: FournisseurRow;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
  submitLabel?: string;
};

export function FournisseurForm({ fournisseur, action, submitLabel = "Enregistrer" }: Props) {
  const [state, formAction, pending] = useActionState(action, initial);

  return (
    <form action={formAction} className="space-y-6">
      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}
      {state.ok && (
        <Alert>
          <AlertDescription>Modifications enregistrées.</AlertDescription>
        </Alert>
      )}

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Identité</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="nom">
              Nom <span className="text-red-600">*</span>
            </Label>
            <Input
              id="nom"
              name="nom"
              defaultValue={fournisseur?.nom ?? ""}
              required
              aria-invalid={Boolean(state.fieldErrors?.nom)}
            />
            {state.fieldErrors?.nom && (
              <p className="text-xs text-red-600">{state.fieldErrors.nom}</p>
            )}
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Adresse</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <FieldText
            label="Ligne 1"
            name="adresse_ligne1"
            defaultValue={fournisseur?.adresse_ligne1 ?? ""}
            wide
          />
          <FieldText
            label="Ligne 2"
            name="adresse_ligne2"
            defaultValue={fournisseur?.adresse_ligne2 ?? ""}
            wide
          />
          <FieldText
            label="Ligne 3"
            name="adresse_ligne3"
            defaultValue={fournisseur?.adresse_ligne3 ?? ""}
            wide
          />
          <FieldText
            label="Code postal"
            name="adresse_code_postal"
            defaultValue={fournisseur?.adresse_code_postal ?? ""}
          />
          <FieldText
            label="Ville"
            name="adresse_ville"
            defaultValue={fournisseur?.adresse_ville ?? ""}
          />
          <FieldText
            label="Pays"
            name="adresse_pays"
            defaultValue={fournisseur?.adresse_pays ?? "France"}
          />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Commentaire</CardTitle>
        </CardHeader>
        <CardContent>
          <Textarea
            name="commentaire"
            defaultValue={fournisseur?.commentaire ?? ""}
            rows={4}
            placeholder="Notes internes (contact commercial, conditions particulières, etc.)"
          />
        </CardContent>
      </Card>

      <div className="flex justify-end">
        <Button type="submit" disabled={pending}>
          {pending ? "Enregistrement…" : submitLabel}
        </Button>
      </div>
    </form>
  );
}

function FieldText({
  label,
  name,
  defaultValue,
  wide,
}: {
  label: string;
  name: string;
  defaultValue?: string | null;
  wide?: boolean;
}) {
  return (
    <div className={"space-y-2" + (wide ? " md:col-span-2" : "")}>
      <Label htmlFor={name}>{label}</Label>
      <Input id={name} name={name} defaultValue={defaultValue ?? ""} />
    </div>
  );
}
