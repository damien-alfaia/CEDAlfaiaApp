import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { RendezVousRow } from "@/lib/db/types";

/**
 * Liste des RDV à venir + ceux du jour. Ordonnés du plus proche au plus lointain.
 */
export async function listRendezVousAVenir(): Promise<RendezVousRow[]> {
  const supabase = await createClient();
  const debutJour = new Date();
  debutJour.setHours(0, 0, 0, 0);
  const { data, error } = await supabase
    .from("rendez_vous")
    .select("*")
    .gte("date_heure_debut", debutJour.toISOString())
    .order("date_heure_debut", { ascending: true })
    .limit(200);
  if (error) throw new Error(`listRendezVousAVenir: ${error.message}`);
  return (data ?? []) as RendezVousRow[];
}

export async function listRendezVousPasses(): Promise<RendezVousRow[]> {
  const supabase = await createClient();
  const debutJour = new Date();
  debutJour.setHours(0, 0, 0, 0);
  const { data, error } = await supabase
    .from("rendez_vous")
    .select("*")
    .lt("date_heure_debut", debutJour.toISOString())
    .order("date_heure_debut", { ascending: false })
    .limit(50);
  if (error) throw new Error(`listRendezVousPasses: ${error.message}`);
  return (data ?? []) as RendezVousRow[];
}

export async function getRendezVous(id: number): Promise<RendezVousRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase.from("rendez_vous").select("*").eq("id", id).maybeSingle();
  if (error) throw new Error(`getRendezVous: ${error.message}`);
  return (data as RendezVousRow | null) ?? null;
}
