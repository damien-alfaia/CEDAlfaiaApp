"use client";

import { useActionState, useMemo, useState } from "react";
import { Trash2, Plus } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { fmtEuro, TYPE_PAIEMENT_LABEL } from "@/lib/format";
import type {
  LigneInput,
  PieceVenteWriteInput,
  ServiceInput,
  PaiementInput,
} from "@/lib/schemas/pieces-vente";
import type { ActionState } from "../_actions/pieces-vente";

type ClientOption = {
  id: number;
  label: string;
  voitures: { id: number; label: string }[];
};
type FournisseurOption = { id: number; label: string };

type Props = {
  mode: "devis" | "facture";
  clients: ClientOption[];
  fournisseurs: FournisseurOption[];
  /** Pré-rempli en édition. */
  initial?: Partial<PieceVenteWriteInput>;
  action: (prev: ActionState, form: FormData) => Promise<ActionState>;
  /** Bouton submit principal. */
  submitLabel?: string;
  /** Action UI supplémentaires (boutons à droite des totaux). */
  extraActions?: React.ReactNode;
};

const initialActionState: ActionState = {};

function emptyLigne(): LigneInput {
  return {
    fournisseur_id: null,
    libelle: "",
    quantite: 1,
    remise: 0,
    prix_garage_ht: 0,
    prix_garage_ttc: 0,
    prix_client_ht: 0,
    prix_client_ttc: 0,
  };
}

function emptyService(): ServiceInput {
  return { libelle: "", quantite: 1, prix_client_ht: 0, prix_client_ttc: 0 };
}

function emptyPaiement(): PaiementInput {
  return {
    type_paiement: "especes",
    date: new Date().toISOString().slice(0, 10),
    montant: 0,
  };
}

export function PieceVenteForm({
  mode,
  clients,
  fournisseurs,
  initial,
  action,
  submitLabel,
  extraActions,
}: Props) {
  const [state, formAction, pending] = useActionState(action, initialActionState);

  const [clientId, setClientId] = useState<string>(initial?.client_id?.toString() ?? "");
  const [voitureId, setVoitureId] = useState<string>(initial?.voiture_id?.toString() ?? "");
  const [date, setDate] = useState<string>(initial?.date ?? new Date().toISOString().slice(0, 10));
  const [kilometrage, setKilometrage] = useState<number>(initial?.kilometrage ?? 0);
  const [moMontant, setMoMontant] = useState<number>(initial?.main_doeuvre_montant_horaire ?? 0);
  const [moDuree, setMoDuree] = useState<number>(initial?.main_doeuvre_duree ?? 1);
  const [remise, setRemise] = useState<string>(
    initial?.remise != null ? String(initial.remise) : "",
  );
  const [isDevisEnvoye, setIsDevisEnvoye] = useState<boolean>(initial?.is_devis_envoye ?? false);
  const [lignes, setLignes] = useState<LigneInput[]>(initial?.lignes ?? []);
  const [services, setServices] = useState<ServiceInput[]>(initial?.services ?? []);
  const [paiements, setPaiements] = useState<PaiementInput[]>(initial?.paiements ?? []);

  const voitures = useMemo(() => {
    const c = clients.find((x) => x.id.toString() === clientId);
    return c?.voitures ?? [];
  }, [clientId, clients]);

  const totaux = useMemo(() => {
    const mo = moMontant * moDuree;
    let totalHt = mo;
    let totalTtc = mo;
    let totalGarageTtc = 0;
    for (const l of lignes) {
      totalHt += l.prix_client_ht * l.quantite;
      totalTtc += l.prix_client_ttc * l.quantite;
      totalGarageTtc += l.prix_garage_ttc * l.quantite;
    }
    for (const s of services) {
      totalHt += s.prix_client_ht * s.quantite;
      totalTtc += s.prix_client_ttc * s.quantite;
    }
    const r = parseFloat(remise.replace(",", ".")) || 0;
    if (r > 0) {
      totalHt *= 1 - r;
      totalTtc *= 1 - r;
    }
    const totalPaiements = paiements.reduce((a, p) => a + (p.montant || 0), 0);
    return {
      total_ht: totalHt,
      total_ttc: totalTtc,
      montant_tva: totalTtc - totalHt,
      benefice_ttc: totalTtc - totalGarageTtc,
      reste_a_payer: totalTtc - totalPaiements,
    };
  }, [lignes, services, paiements, moMontant, moDuree, remise]);

  const payload: PieceVenteWriteInput = {
    type_piece: mode,
    client_id: Number(clientId) || 0,
    voiture_id: Number(voitureId) || 0,
    date,
    kilometrage: kilometrage || 0,
    main_doeuvre_montant_horaire: moMontant,
    main_doeuvre_duree: moDuree,
    remise: remise === "" ? null : parseFloat(remise.replace(",", ".")),
    is_devis_envoye: isDevisEnvoye,
    lignes,
    services,
    paiements,
  };

  return (
    <form action={formAction} className="space-y-6">
      <input type="hidden" name="payload" value={JSON.stringify(payload)} />

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
          <CardTitle className="text-base">Client &amp; véhicule</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <div className="space-y-2">
            <Label>
              Client <span className="text-red-600">*</span>
            </Label>
            <Select
              value={clientId}
              onValueChange={(v) => {
                setClientId(v);
                setVoitureId("");
              }}
            >
              <SelectTrigger>
                <SelectValue placeholder="Choisir un client…" />
              </SelectTrigger>
              <SelectContent>
                {clients.map((c) => (
                  <SelectItem key={c.id} value={c.id.toString()}>
                    {c.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {state.fieldErrors?.client_id && (
              <p className="text-xs text-red-600">{state.fieldErrors.client_id}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label>
              Voiture <span className="text-red-600">*</span>
            </Label>
            <Select value={voitureId} onValueChange={setVoitureId} disabled={!clientId}>
              <SelectTrigger>
                <SelectValue placeholder="Choisir une voiture…" />
              </SelectTrigger>
              <SelectContent>
                {voitures.length === 0 && (
                  <div className="px-2 py-2 text-sm text-muted-foreground">
                    Aucune voiture pour ce client.
                  </div>
                )}
                {voitures.map((v) => (
                  <SelectItem key={v.id} value={v.id.toString()}>
                    {v.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {state.fieldErrors?.voiture_id && (
              <p className="text-xs text-red-600">{state.fieldErrors.voiture_id}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="date">
              Date <span className="text-red-600">*</span>
            </Label>
            <Input id="date" type="date" value={date} onChange={(e) => setDate(e.target.value)} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="kilometrage">Kilométrage</Label>
            <Input
              id="kilometrage"
              type="number"
              min={0}
              value={kilometrage}
              onChange={(e) => setKilometrage(Number(e.target.value) || 0)}
            />
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Main d&apos;œuvre</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-3">
          <div className="space-y-2">
            <Label htmlFor="mo_montant">Montant horaire (€)</Label>
            <Input
              id="mo_montant"
              type="number"
              step="0.01"
              min={0}
              value={moMontant}
              onChange={(e) => setMoMontant(parseFloat(e.target.value) || 0)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="mo_duree">Durée (h)</Label>
            <Input
              id="mo_duree"
              type="number"
              min={0}
              value={moDuree}
              onChange={(e) => setMoDuree(parseInt(e.target.value, 10) || 0)}
            />
          </div>
          <div className="space-y-2">
            <Label>Total main d&apos;œuvre</Label>
            <div className="flex h-9 items-center rounded-md border bg-muted/40 px-3 text-sm tabular-nums">
              {fmtEuro(moMontant * moDuree)}
            </div>
          </div>
        </CardContent>
      </Card>

      <LignesSection lignes={lignes} setLignes={setLignes} fournisseurs={fournisseurs} />

      <ServicesSection services={services} setServices={setServices} />

      {mode === "facture" && <PaiementsSection paiements={paiements} setPaiements={setPaiements} />}

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Récapitulatif</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="grid gap-4 md:grid-cols-3">
            <div className="space-y-2">
              <Label htmlFor="remise">Remise globale (0–1, ex 0.05 = 5%)</Label>
              <Input
                id="remise"
                inputMode="decimal"
                placeholder="0"
                value={remise}
                onChange={(e) => setRemise(e.target.value)}
              />
            </div>
            {mode === "devis" && (
              <div className="flex items-end gap-2">
                <Checkbox
                  id="is_devis_envoye"
                  checked={isDevisEnvoye}
                  onCheckedChange={(v) => setIsDevisEnvoye(v === true)}
                />
                <Label htmlFor="is_devis_envoye" className="font-normal">
                  Devis envoyé au client
                </Label>
              </div>
            )}
          </div>
          <Separator />
          <dl className="grid gap-1 text-sm md:grid-cols-2">
            <Stat label="Total HT" value={fmtEuro(totaux.total_ht)} />
            <Stat label="Total TTC" value={fmtEuro(totaux.total_ttc)} bold />
            <Stat label="Montant TVA" value={fmtEuro(totaux.montant_tva)} />
            <Stat label="Bénéfice TTC" value={fmtEuro(totaux.benefice_ttc)} />
            {mode === "facture" && (
              <Stat
                label="Reste à payer"
                value={fmtEuro(totaux.reste_a_payer)}
                bold
                emphasis={
                  totaux.reste_a_payer === 0
                    ? "ok"
                    : totaux.reste_a_payer === totaux.total_ttc
                      ? "alert"
                      : "warn"
                }
              />
            )}
          </dl>
        </CardContent>
      </Card>

      <div className="flex flex-wrap items-center justify-end gap-2">
        {extraActions}
        <Button type="submit" disabled={pending}>
          {pending ? "Enregistrement…" : (submitLabel ?? "Enregistrer")}
        </Button>
      </div>
    </form>
  );
}

function Stat({
  label,
  value,
  bold,
  emphasis,
}: {
  label: string;
  value: string;
  bold?: boolean;
  emphasis?: "ok" | "warn" | "alert";
}) {
  const color =
    emphasis === "ok"
      ? "text-green-600"
      : emphasis === "alert"
        ? "text-red-600"
        : emphasis === "warn"
          ? "text-amber-600"
          : "";
  return (
    <div className="flex items-center justify-between rounded-md px-3 py-1.5 hover:bg-muted/40">
      <dt className="text-muted-foreground">{label}</dt>
      <dd className={`tabular-nums ${bold ? "font-semibold" : ""} ${color}`}>{value}</dd>
    </div>
  );
}

// =============================================================================
// Sections — Lignes / Services / Paiements
// =============================================================================

function LignesSection({
  lignes,
  setLignes,
  fournisseurs,
}: {
  lignes: LigneInput[];
  setLignes: React.Dispatch<React.SetStateAction<LigneInput[]>>;
  fournisseurs: FournisseurOption[];
}) {
  const update = (i: number, patch: Partial<LigneInput>) =>
    setLignes((arr) => arr.map((l, idx) => (idx === i ? { ...l, ...patch } : l)));
  const add = () => setLignes((arr) => [...arr, emptyLigne()]);
  const remove = (i: number) => setLignes((arr) => arr.filter((_, idx) => idx !== i));

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0">
        <CardTitle className="text-base">Pièces &amp; fournitures</CardTitle>
        <Button type="button" size="sm" variant="outline" onClick={add}>
          <Plus className="mr-1 h-4 w-4" />
          Ajouter une ligne
        </Button>
      </CardHeader>
      <CardContent>
        {lignes.length === 0 ? (
          <p className="text-sm text-muted-foreground">Aucune ligne.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-xs uppercase text-muted-foreground">
                  <th className="px-2 py-1">Libellé</th>
                  <th className="px-2 py-1">Fournisseur</th>
                  <th className="px-2 py-1 text-right">Qté</th>
                  <th className="px-2 py-1 text-right">Garage HT</th>
                  <th className="px-2 py-1 text-right">Garage TTC</th>
                  <th className="px-2 py-1 text-right">Client HT</th>
                  <th className="px-2 py-1 text-right">Client TTC</th>
                  <th className="px-2 py-1 text-right">Total TTC</th>
                  <th className="px-2 py-1"></th>
                </tr>
              </thead>
              <tbody>
                {lignes.map((l, i) => (
                  <tr key={i} className="border-t">
                    <td className="px-1 py-1">
                      <Input
                        value={l.libelle}
                        onChange={(e) => update(i, { libelle: e.target.value })}
                        placeholder="Libellé"
                      />
                    </td>
                    <td className="px-1 py-1 min-w-[160px]">
                      <Select
                        value={l.fournisseur_id ? l.fournisseur_id.toString() : "none"}
                        onValueChange={(v) =>
                          update(i, { fournisseur_id: v === "none" ? null : Number(v) })
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="—" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="none">—</SelectItem>
                          {fournisseurs.map((f) => (
                            <SelectItem key={f.id} value={f.id.toString()}>
                              {f.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </td>
                    <td className="px-1 py-1 w-20">
                      <Input
                        className="text-right"
                        type="number"
                        min={1}
                        value={l.quantite}
                        onChange={(e) => update(i, { quantite: parseInt(e.target.value, 10) || 1 })}
                      />
                    </td>
                    <td className="px-1 py-1 w-28">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={l.prix_garage_ht}
                        onChange={(e) =>
                          update(i, { prix_garage_ht: parseFloat(e.target.value) || 0 })
                        }
                      />
                    </td>
                    <td className="px-1 py-1 w-28">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={l.prix_garage_ttc}
                        onChange={(e) =>
                          update(i, { prix_garage_ttc: parseFloat(e.target.value) || 0 })
                        }
                      />
                    </td>
                    <td className="px-1 py-1 w-28">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={l.prix_client_ht}
                        onChange={(e) =>
                          update(i, { prix_client_ht: parseFloat(e.target.value) || 0 })
                        }
                      />
                    </td>
                    <td className="px-1 py-1 w-28">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={l.prix_client_ttc}
                        onChange={(e) =>
                          update(i, { prix_client_ttc: parseFloat(e.target.value) || 0 })
                        }
                      />
                    </td>
                    <td className="px-2 py-1 text-right tabular-nums">
                      {fmtEuro(l.prix_client_ttc * l.quantite)}
                    </td>
                    <td className="px-1 py-1">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        onClick={() => remove(i)}
                        aria-label="Retirer"
                      >
                        <Trash2 className="h-4 w-4 text-red-600" />
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}

function ServicesSection({
  services,
  setServices,
}: {
  services: ServiceInput[];
  setServices: React.Dispatch<React.SetStateAction<ServiceInput[]>>;
}) {
  const update = (i: number, patch: Partial<ServiceInput>) =>
    setServices((arr) => arr.map((s, idx) => (idx === i ? { ...s, ...patch } : s)));
  const add = () => setServices((arr) => [...arr, emptyService()]);
  const remove = (i: number) => setServices((arr) => arr.filter((_, idx) => idx !== i));

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0">
        <CardTitle className="text-base">Services</CardTitle>
        <Button type="button" size="sm" variant="outline" onClick={add}>
          <Plus className="mr-1 h-4 w-4" />
          Ajouter un service
        </Button>
      </CardHeader>
      <CardContent>
        {services.length === 0 ? (
          <p className="text-sm text-muted-foreground">Aucun service.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-xs uppercase text-muted-foreground">
                  <th className="px-2 py-1">Libellé</th>
                  <th className="px-2 py-1 text-right">Qté</th>
                  <th className="px-2 py-1 text-right">PU HT</th>
                  <th className="px-2 py-1 text-right">PU TTC</th>
                  <th className="px-2 py-1 text-right">Total TTC</th>
                  <th className="px-2 py-1"></th>
                </tr>
              </thead>
              <tbody>
                {services.map((s, i) => (
                  <tr key={i} className="border-t">
                    <td className="px-1 py-1">
                      <Input
                        value={s.libelle}
                        onChange={(e) => update(i, { libelle: e.target.value })}
                      />
                    </td>
                    <td className="px-1 py-1 w-20">
                      <Input
                        className="text-right"
                        type="number"
                        min={1}
                        value={s.quantite}
                        onChange={(e) => update(i, { quantite: parseInt(e.target.value, 10) || 1 })}
                      />
                    </td>
                    <td className="px-1 py-1 w-28">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={s.prix_client_ht}
                        onChange={(e) =>
                          update(i, { prix_client_ht: parseFloat(e.target.value) || 0 })
                        }
                      />
                    </td>
                    <td className="px-1 py-1 w-28">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={s.prix_client_ttc}
                        onChange={(e) =>
                          update(i, { prix_client_ttc: parseFloat(e.target.value) || 0 })
                        }
                      />
                    </td>
                    <td className="px-2 py-1 text-right tabular-nums">
                      {fmtEuro(s.prix_client_ttc * s.quantite)}
                    </td>
                    <td className="px-1 py-1">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        onClick={() => remove(i)}
                        aria-label="Retirer"
                      >
                        <Trash2 className="h-4 w-4 text-red-600" />
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}

function PaiementsSection({
  paiements,
  setPaiements,
}: {
  paiements: PaiementInput[];
  setPaiements: React.Dispatch<React.SetStateAction<PaiementInput[]>>;
}) {
  const update = (i: number, patch: Partial<PaiementInput>) =>
    setPaiements((arr) => arr.map((p, idx) => (idx === i ? { ...p, ...patch } : p)));
  const add = () => setPaiements((arr) => [...arr, emptyPaiement()]);
  const remove = (i: number) => setPaiements((arr) => arr.filter((_, idx) => idx !== i));

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0">
        <CardTitle className="text-base">Paiements</CardTitle>
        <Button type="button" size="sm" variant="outline" onClick={add}>
          <Plus className="mr-1 h-4 w-4" />
          Ajouter un paiement
        </Button>
      </CardHeader>
      <CardContent>
        {paiements.length === 0 ? (
          <p className="text-sm text-muted-foreground">Aucun paiement.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-xs uppercase text-muted-foreground">
                  <th className="px-2 py-1">Mode</th>
                  <th className="px-2 py-1">Date</th>
                  <th className="px-2 py-1 text-right">Montant</th>
                  <th className="px-2 py-1"></th>
                </tr>
              </thead>
              <tbody>
                {paiements.map((p, i) => (
                  <tr key={i} className="border-t">
                    <td className="px-1 py-1 min-w-[160px]">
                      <Select
                        value={p.type_paiement}
                        onValueChange={(v) =>
                          update(i, { type_paiement: v as PaiementInput["type_paiement"] })
                        }
                      >
                        <SelectTrigger>
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {(["especes", "cheque", "cb", "virement"] as const).map((m) => (
                            <SelectItem key={m} value={m}>
                              {TYPE_PAIEMENT_LABEL[m]}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </td>
                    <td className="px-1 py-1 w-44">
                      <Input
                        type="date"
                        value={p.date}
                        onChange={(e) => update(i, { date: e.target.value })}
                      />
                    </td>
                    <td className="px-1 py-1 w-32">
                      <Input
                        className="text-right"
                        type="number"
                        step="0.01"
                        value={p.montant}
                        onChange={(e) => update(i, { montant: parseFloat(e.target.value) || 0 })}
                      />
                    </td>
                    <td className="px-1 py-1">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        onClick={() => remove(i)}
                        aria-label="Retirer"
                      >
                        <Trash2 className="h-4 w-4 text-red-600" />
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
