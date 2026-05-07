import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { SalarieRow } from "@/lib/db/types";

export async function listSalaries(): Promise<SalarieRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase.from("salaries").select("*").order("nom").order("prenom");
  if (error) throw new Error(`listSalaries: ${error.message}`);
  return (data ?? []) as SalarieRow[];
}

export async function getSalarie(id: number): Promise<SalarieRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase.from("salaries").select("*").eq("id", id).maybeSingle();
  if (error) throw new Error(`getSalarie: ${error.message}`);
  return (data as SalarieRow | null) ?? null;
}
