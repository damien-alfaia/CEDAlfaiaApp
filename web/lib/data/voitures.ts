import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { VoitureRow, VoitureWithModele } from "@/lib/db/types";

const VOITURE_WITH_MODELE_SELECT = `
  id, immatriculation, modele_id, client_id, is_principale,
  date_suppression, created_at, updated_at,
  modele:modeles ( id, libelle, marque:marques ( id, libelle ) )
`;

export async function listVoituresByClient(clientId: number): Promise<VoitureWithModele[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("voitures")
    .select(VOITURE_WITH_MODELE_SELECT)
    .eq("client_id", clientId)
    .is("date_suppression", null)
    .order("is_principale", { ascending: false })
    .order("created_at", { ascending: true });
  if (error) throw new Error(`listVoituresByClient: ${error.message}`);
  return (data ?? []) as unknown as VoitureWithModele[];
}

export async function getVoiture(id: number): Promise<VoitureWithModele | null> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("voitures")
    .select(VOITURE_WITH_MODELE_SELECT)
    .eq("id", id)
    .maybeSingle();
  if (error) throw new Error(`getVoiture: ${error.message}`);
  return (data as unknown as VoitureWithModele | null) ?? null;
}

export async function getVoitureRow(id: number): Promise<VoitureRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase.from("voitures").select("*").eq("id", id).maybeSingle();
  if (error) throw new Error(`getVoitureRow: ${error.message}`);
  return (data as VoitureRow | null) ?? null;
}
