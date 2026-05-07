"use client";

import { useActionState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Checkbox } from "@/components/ui/checkbox";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { ClientRow } from "@/lib/db/types";
import type { ActionState } from "../actions";

type Props = {
  client?: ClientRow;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
  submitLabel?: string;
};

const initial: ActionState = {};

export function ClientForm({ client, action, submitLabel = "Enregistrer" }: Props) {
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
          <Field label="Nom" name="nom" required defaultValue={client?.nom} error={fe["nom"]} />
          <Field label="Prénom" name="prenom" defaultValue={client?.prenom ?? ""} />
          <Field
            label="Code"
            name="code"
            defaultValue={client?.code ?? ""}
            placeholder="ex: AUT001"
          />
          <div className="flex items-end gap-2">
            <Checkbox
              id="is_prospect"
              name="is_prospect"
              defaultChecked={client ? client.is_prospect : true}
            />
            <Label htmlFor="is_prospect" className="font-normal">
              Prospect (non encore facturé)
            </Label>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Contact</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field
            label="Téléphone"
            name="telephone"
            defaultValue={client?.telephone ?? ""}
            type="tel"
          />
          <Field
            label="Email"
            name="email"
            defaultValue={client?.email ?? ""}
            type="email"
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
            defaultValue={client?.adresse_ligne1 ?? ""}
            wide
          />
          <Field
            label="Ligne 2"
            name="adresse_ligne2"
            defaultValue={client?.adresse_ligne2 ?? ""}
            wide
          />
          <Field
            label="Ligne 3"
            name="adresse_ligne3"
            defaultValue={client?.adresse_ligne3 ?? ""}
            wide
          />
          <Field
            label="Code postal"
            name="adresse_code_postal"
            defaultValue={client?.adresse_code_postal ?? ""}
          />
          <Field label="Ville" name="adresse_ville" defaultValue={client?.adresse_ville ?? ""} />
          <Field label="Pays" name="adresse_pays" defaultValue={client?.adresse_pays ?? "France"} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Commercial</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field
            label="Remise par défaut (0–1, ex 0.05 = 5%)"
            name="remise"
            defaultValue={client?.remise?.toString() ?? ""}
            placeholder="ex: 0.05"
            error={fe["remise"]}
          />
          <div className="md:col-span-2 space-y-2">
            <Label htmlFor="informations_complementaires">Informations complémentaires</Label>
            <Textarea
              id="informations_complementaires"
              name="informations_complementaires"
              defaultValue={client?.informations_complementaires ?? ""}
              rows={4}
            />
          </div>
        </CardContent>
      </Card>

      <div className="flex justify-end gap-2">
        <Button type="submit" disabled={pending}>
          {pending ? "Enregistrement…" : submitLabel}
        </Button>
      </div>
    </form>
  );
}

type FieldProps = {
  label: string;
  name: string;
  defaultValue?: string | null;
  required?: boolean;
  type?: string;
  placeholder?: string;
  wide?: boolean;
  error?: string;
};

function Field({
  label,
  name,
  defaultValue,
  required,
  type,
  placeholder,
  wide,
  error,
}: FieldProps) {
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
        placeholder={placeholder}
        aria-invalid={Boolean(error)}
      />
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}
