import "server-only";
import { createClient } from "@/lib/supabase/server";
import type {
  SalarieContratRow,
  SalarieIndisponibiliteRow,
  SalarieSalaireRow,
} from "@/lib/db/types";

// =============================================================================
// Contrats
// =============================================================================

export async function listContratsForSalarie(salarieId: number): Promise<SalarieContratRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("salarie_contrats")
    .select("*")
    .eq("salarie_id", salarieId)
    .order("date_debut", { ascending: false });
  if (error) throw new Error(`listContratsForSalarie: ${error.message}`);
  return (data ?? []) as SalarieContratRow[];
}

export async function getContrat(id: number): Promise<SalarieContratRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("salarie_contrats")
    .select("*")
    .eq("id", id)
    .maybeSingle();
  if (error) throw new Error(`getContrat: ${error.message}`);
  return (data as SalarieContratRow | null) ?? null;
}

// =============================================================================
// Indisponibilités
// =============================================================================

export async function listIndisponibilitesForContrat(
  contratId: number,
): Promise<SalarieIndisponibiliteRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("salarie_indisponibilites")
    .select("*")
    .eq("salarie_contrat_id", contratId)
    .order("date_debut", { ascending: false });
  if (error) throw new Error(`listIndisponibilitesForContrat: ${error.message}`);
  return (data ?? []) as SalarieIndisponibiliteRow[];
}

// =============================================================================
// Bulletins de salaire
// =============================================================================

export async function listSalairesForContrat(contratId: number): Promise<SalarieSalaireRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("salarie_salaires")
    .select("*")
    .eq("salarie_contrat_id", contratId)
    .order("date_paiement", { ascending: false });
  if (error) throw new Error(`listSalairesForContrat: ${error.message}`);
  return (data ?? []) as SalarieSalaireRow[];
}
