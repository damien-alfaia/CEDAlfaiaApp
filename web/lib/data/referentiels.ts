import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { MarqueRow, MarqueWithModeles, ModeleRow } from "@/lib/db/types";

export async function listMarques(): Promise<MarqueRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase.from("marques").select("*").order("libelle");
  if (error) throw new Error(`listMarques: ${error.message}`);
  return (data ?? []) as MarqueRow[];
}

export async function listMarquesWithModeles(): Promise<MarqueWithModeles[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("marques")
    .select("*, modeles ( * )")
    .order("libelle");
  if (error) throw new Error(`listMarquesWithModeles: ${error.message}`);
  // Tri des modeles côté JS pour être sûr
  return ((data ?? []) as unknown as MarqueWithModeles[]).map((m) => ({
    ...m,
    modeles: [...(m.modeles ?? [])].sort((a, b) => a.libelle.localeCompare(b.libelle)),
  }));
}

export async function listModelesByMarque(marqueId: number): Promise<ModeleRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("modeles")
    .select("*")
    .eq("marque_id", marqueId)
    .order("libelle");
  if (error) throw new Error(`listModelesByMarque: ${error.message}`);
  return (data ?? []) as ModeleRow[];
}

export async function listAllModelesGrouped(): Promise<
  { id: number; label: string; marqueLibelle: string }[]
> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("modeles")
    .select("id, libelle, marque:marques ( libelle )")
    .order("libelle");
  if (error) throw new Error(`listAllModelesGrouped: ${error.message}`);
  type Row = { id: number; libelle: string; marque: { libelle: string } | null };
  return ((data ?? []) as unknown as Row[]).map((r) => ({
    id: r.id,
    label: r.libelle,
    marqueLibelle: r.marque?.libelle ?? "—",
  }));
}
