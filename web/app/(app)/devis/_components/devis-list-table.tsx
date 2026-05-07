"use client";

import Link from "next/link";
import { useState } from "react";
import { Check, FileDown, X } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { fmtDate, fmtEuro, fmtNumero } from "@/lib/format";
import type { PieceVenteListRow } from "@/lib/db/types";

type Props = {
  rows: PieceVenteListRow[];
};

export function DevisListTable({ rows }: Props) {
  const [selected, setSelected] = useState<Set<number>>(new Set());

  if (rows.length === 0) {
    return (
      <div className="rounded-md border border-dashed p-8 text-center text-sm text-muted-foreground">
        Aucun devis à afficher.
      </div>
    );
  }

  const allSelected = rows.length > 0 && rows.every((r) => selected.has(r.id));
  const someSelected = !allSelected && rows.some((r) => selected.has(r.id));

  const toggle = (id: number) => {
    setSelected((s) => {
      const next = new Set(s);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };
  const toggleAll = () => {
    setSelected((s) => {
      if (allSelected) return new Set();
      const next = new Set(s);
      for (const r of rows) next.add(r.id);
      return next;
    });
  };

  const printSelection = () => {
    if (selected.size === 0) return;
    const ids = Array.from(selected).join(",");
    window.open(`/devis/pdf-batch?ids=${ids}`, "_blank", "noopener,noreferrer");
  };

  return (
    <div className="space-y-3">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <p className="text-sm text-muted-foreground">
          {selected.size > 0 ? `${selected.size} sélectionné(s)` : "Aucune sélection"}
        </p>
        <Button onClick={printSelection} disabled={selected.size === 0} size="sm">
          <FileDown className="mr-2 h-4 w-4" />
          Imprimer la sélection ({selected.size})
        </Button>
      </div>

      <div className="overflow-x-auto rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-10">
                <Checkbox
                  checked={allSelected ? true : someSelected ? "indeterminate" : false}
                  onCheckedChange={toggleAll}
                  aria-label="Sélectionner tout"
                />
              </TableHead>
              <TableHead>N°</TableHead>
              <TableHead className="hidden sm:table-cell">Date</TableHead>
              <TableHead>Client</TableHead>
              <TableHead className="hidden md:table-cell">Voiture</TableHead>
              <TableHead className="text-right">Total TTC</TableHead>
              <TableHead className="text-center">Envoyé</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {rows.map((d) => {
              const fullName = [d.client?.prenom, d.client?.nom].filter(Boolean).join(" ").trim();
              const voiture =
                d.voiture?.modele?.marque?.libelle && d.voiture?.modele?.libelle
                  ? `${d.voiture.modele.marque.libelle} ${d.voiture.modele.libelle}`
                  : "—";
              return (
                <TableRow key={d.id} data-state={selected.has(d.id) ? "selected" : undefined}>
                  <TableCell className="w-10">
                    <Checkbox
                      checked={selected.has(d.id)}
                      onCheckedChange={() => toggle(d.id)}
                      aria-label={`Sélectionner ${fmtNumero(d.num_devis)}`}
                    />
                  </TableCell>
                  <TableCell className="font-mono">
                    <Link href={`/devis/${d.id}`} className="hover:underline">
                      {fmtNumero(d.num_devis)}
                    </Link>
                  </TableCell>
                  <TableCell className="hidden sm:table-cell">{fmtDate(d.date_devis)}</TableCell>
                  <TableCell>{fullName || "—"}</TableCell>
                  <TableCell className="hidden md:table-cell">
                    {voiture}
                    {d.voiture?.immatriculation && (
                      <span className="ml-1 font-mono text-xs text-muted-foreground/50">
                        {d.voiture.immatriculation}
                      </span>
                    )}
                  </TableCell>
                  <TableCell className="text-right tabular-nums">{fmtEuro(d.total_ttc)}</TableCell>
                  <TableCell className="text-center">
                    {d.is_devis_envoye ? (
                      <Check className="mx-auto h-4 w-4 text-green-600" />
                    ) : (
                      <X className="mx-auto h-4 w-4 text-muted-foreground/50" />
                    )}
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
