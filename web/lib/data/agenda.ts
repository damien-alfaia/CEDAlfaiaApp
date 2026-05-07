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

/**
 * Tous les RDV qui chevauchent le mois donné (year + month 1-12), avec un peu
 * de marge pour capter les semaines qui débordent côté grille calendrier.
 */
export async function listRendezVousMois(year: number, month: number): Promise<RendezVousRow[]> {
  const supabase = await createClient();
  const start = new Date(year, month - 2, 1, 0, 0, 0, 0);
  const end = new Date(year, month + 1, 1, 0, 0, 0, 0);
  const { data, error } = await supabase
    .from("rendez_vous")
    .select("*")
    .gte("date_heure_debut", start.toISOString())
    .lt("date_heure_debut", end.toISOString())
    .order("date_heure_debut", { ascending: true });
  if (error) throw new Error(`listRendezVousMois: ${error.message}`);
  return (data ?? []) as RendezVousRow[];
}
