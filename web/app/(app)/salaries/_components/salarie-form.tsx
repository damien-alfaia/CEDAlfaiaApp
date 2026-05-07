"use client";

import { useActionState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { SalarieRow } from "@/lib/db/types";
import type { ActionState } from "../actions";

const initial: ActionState = {};

type Props = {
  salarie?: SalarieRow;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
  submitLabel?: string;
};

export function SalarieForm({ salarie, action, submitLabel = "Enregistrer" }: Props) {
  const [state, formAction, pending] = useActionState(action, initial);
  const fe = state.fieldErrors ?? {};

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
          <Field label="Nom" name="nom" defaultValue={salarie?.nom} required error={fe["nom"]} />
          <Field label="Prénom" name="prenom" defaultValue={salarie?.prenom} />
          <Field
            label="Date de naissance"
            name="date_naissance"
            type="date"
            defaultValue={salarie?.date_naissance ?? ""}
            required
            error={fe["date_naissance"]}
          />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Contact</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field label="Téléphone" name="telephone" defaultValue={salarie?.telephone} type="tel" />
          <Field label="Portable" name="portable" defaultValue={salarie?.portable} type="tel" />
          <Field
            label="Email"
            name="email"
            defaultValue={salarie?.email}
            type="email"
            wide
            error={fe["email"]}
          />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Adresse</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field
            label="Ligne 1"
            name="adresse_ligne1"
            defaultValue={salarie?.adresse_ligne1}
            wide
          />
          <Field
            label="Ligne 2"
            name="adresse_ligne2"
            defaultValue={salarie?.adresse_ligne2}
            wide
          />
          <Field
            label="Code postal"
            name="adresse_code_postal"
            defaultValue={salarie?.adresse_code_postal}
          />
          <Field label="Ville" name="adresse_ville" defaultValue={salarie?.adresse_ville} />
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

function Field({
  label,
  name,
  defaultValue,
  type,
  required,
  wide,
  error,
}: {
  label: string;
  name: string;
  defaultValue?: string | null | undefined;
  type?: string;
  required?: boolean;
  wide?: boolean;
  error?: string;
}) {
  return (
    <div className={"space-y-2" + (wide ? " md:col-span-2" : "")}>
      <Label htmlFor={name}>
        {label}
        {required && <span className="text-red-600"> *</span>}
      </Label>
      <Input
        id={name}
        name={name}
        type={type ?? "text"}
        defaultValue={defaultValue ?? ""}
        required={required}
        aria-invalid={Boolean(error)}
      />
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}
