import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { FournisseurRow } from "@/lib/db/types";

export type FournisseursListParams = {
  q?: string;
  showDeleted?: boolean;
};

export async function listFournisseurs(
  params: FournisseursListParams = {},
): Promise<FournisseurRow[]> {
  const supabase = await createClient();
  let query = supabase.from("fournisseurs").select("*").order("nom");
  if (!params.showDeleted) query = query.is("date_suppression", null);
  const q = params.q?.trim();
  if (q) {
    const safe = q.replace(/[,()]/g, "");
    query = query.ilike("nom", `%${safe}%`);
  }
  const { data, error } = await query;
  if (error) throw new Error(`listFournisseurs: ${error.message}`);
  return (data ?? []) as FournisseurRow[];
}

export async function getFournisseur(id: number): Promise<FournisseurRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("fournisseurs")
    .select("*")
    .eq("id", id)
    .maybeSingle();
  if (error) throw new Error(`getFournisseur: ${error.message}`);
  return (data as FournisseurRow | null) ?? null;
}
