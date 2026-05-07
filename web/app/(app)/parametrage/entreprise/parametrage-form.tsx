"use client";

import { useActionState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Checkbox } from "@/components/ui/checkbox";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { ParametrageRow } from "@/lib/db/types";
import type { ActionState } from "./actions";

const initial: ActionState = {};

type Props = {
  parametrage: ParametrageRow | null;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
};

export function ParametrageForm({ parametrage, action }: Props) {
  const [state, formAction, pending] = useActionState(action, initial);
  const p = parametrage;

  return (
    <form action={formAction} className="space-y-6">
      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}
      {state.ok && (
        <Alert>
          <AlertDescription>Paramètres enregistrés.</AlertDescription>
        </Alert>
      )}

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Identité de l&apos;entreprise</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field
            label="Nom de l'entreprise"
            name="nom_entreprise"
            defaultValue={p?.nom_entreprise}
            wide
          />
          <Field
            label="Téléphone fixe"
            name="telephone_entreprise"
            defaultValue={p?.telephone_entreprise}
            type="tel"
          />
          <Field
            label="Téléphone portable"
            name="portable_entreprise"
            defaultValue={p?.portable_entreprise}
            type="tel"
          />
          <Field
            label="Email entreprise"
            name="email_entreprise"
            defaultValue={p?.email_entreprise}
            type="email"
            wide
          />
          <Field label="SIRET" name="siret" defaultValue={p?.siret} />
          <Field label="Code APE" name="code_ape" defaultValue={p?.code_ape} />
          <Field
            label="N° TVA intracommunautaire"
            name="tva_intra_communautaire"
            defaultValue={p?.tva_intra_communautaire}
            wide
          />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Adresse</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field label="Ligne 1" name="adresse_ligne1" defaultValue={p?.adresse_ligne1} wide />
          <Field label="Ligne 2" name="adresse_ligne2" defaultValue={p?.adresse_ligne2} wide />
          <Field label="Ligne 3" name="adresse_ligne3" defaultValue={p?.adresse_ligne3} wide />
          <Field
            label="Code postal"
            name="adresse_code_postal"
            defaultValue={p?.adresse_code_postal}
          />
          <Field label="Ville" name="adresse_ville" defaultValue={p?.adresse_ville} />
          <Field label="Pays" name="adresse_pays" defaultValue={p?.adresse_pays ?? "France"} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Paramètres commerciaux</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field
            label="Taux de TVA (ex 0.20 = 20%)"
            name="tva"
            defaultValue={p?.tva?.toString() ?? "0.20"}
          />
          <Field
            label="Montant horaire main d'œuvre (€)"
            name="main_doeuvre_montant_horaire"
            defaultValue={p?.main_doeuvre_montant_horaire?.toString() ?? "0"}
          />
          <Field
            label="Email comptable"
            name="email_comptable"
            defaultValue={p?.email_comptable}
            type="email"
            wide
          />
          <Field
            label="Objectif annuel CA TTC (€)"
            name="objectif_annuel"
            defaultValue={p?.objectif_annuel?.toString() ?? ""}
            wide
          />
          <div className="md:col-span-2 space-y-2">
            <Label htmlFor="libelle_bas_de_page">
              Libellé bas de page (PDF — mentions légales)
            </Label>
            <Textarea
              id="libelle_bas_de_page"
              name="libelle_bas_de_page"
              defaultValue={p?.libelle_bas_de_page ?? ""}
              rows={3}
              placeholder="ex: SARL au capital de 7 500 € — RCS XXX 123 456 789 — TVA FR..."
            />
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">SMTP (envoi email comptable)</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <Field label="Serveur SMTP" name="serveur_smtp" defaultValue={null} wide />
          <Field label="Port SMTP" name="port_smtp" defaultValue={null} placeholder="465 / 587" />
          <Field label="Email expéditeur" name="email_smtp" defaultValue={null} type="email" />
          <div className="flex items-end gap-2">
            <Checkbox id="is_ssl" name="is_ssl" />
            <Label htmlFor="is_ssl" className="font-normal">
              Activer SSL
            </Label>
          </div>
          <p className="md:col-span-2 text-xs text-neutral-500">
            Le mot de passe SMTP n&apos;est pas géré ici (à passer en variable d&apos;env si envoi
            automatique nécessaire). Pour l&apos;instant, l&apos;envoi se fait via le client mail
            local de l&apos;utilisateur (mailto:).
          </p>
        </CardContent>
      </Card>

      <div className="flex justify-end">
        <Button type="submit" disabled={pending}>
          {pending ? "Enregistrement…" : "Enregistrer"}
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
  wide,
  placeholder,
}: {
  label: string;
  name: string;
  defaultValue?: string | number | null | undefined;
  type?: string;
  wide?: boolean;
  placeholder?: string;
}) {
  const v = defaultValue == null ? "" : String(defaultValue);
  return (
    <div className={"space-y-2" + (wide ? " md:col-span-2" : "")}>
      <Label htmlFor={name}>{label}</Label>
      <Input
        id={name}
        name={name}
        type={type ?? "text"}
        defaultValue={v}
        placeholder={placeholder}
      />
    </div>
  );
}
