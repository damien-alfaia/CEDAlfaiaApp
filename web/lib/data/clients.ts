import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { ClientRow } from "@/lib/db/types";

export type ClientFilter = "actif" | "supprime" | "all";
export type ClientProspectFilter = "tous" | "prospect" | "client";

export type ClientsListParams = {
  q?: string;
  filter?: ClientFilter;
  prospect?: ClientProspectFilter;
  page?: number;
  perPage?: number;
};

export type ClientsListResult = {
  rows: ClientRow[];
  total: number;
  page: number;
  perPage: number;
  pageCount: number;
};

const DEFAULT_PER_PAGE = 25;

/**
 * Liste paginée + recherche + filtres.
 *
 * Recherche : ilike sur nom, prenom, email, telephone, code (OR).
 * Filtre actif/supprimé : sur date_suppression.
 * Filtre prospect/client : sur is_prospect.
 */
export async function listClients(params: ClientsListParams = {}): Promise<ClientsListResult> {
  const supabase = await createClient();
  const page = Math.max(1, params.page ?? 1);
  const perPage = Math.min(100, Math.max(1, params.perPage ?? DEFAULT_PER_PAGE));
  const filter: ClientFilter = params.filter ?? "actif";
  const prospect: ClientProspectFilter = params.prospect ?? "tous";

  let query = supabase.from("clients").select("*", { count: "exact" });

  if (filter === "actif") {
    query = query.is("date_suppression", null);
  } else if (filter === "supprime") {
    query = query.not("date_suppression", "is", null);
  }

  if (prospect === "prospect") {
    query = query.eq("is_prospect", true);
  } else if (prospect === "client") {
    query = query.eq("is_prospect", false);
  }

  const q = params.q?.trim();
  if (q && q.length > 0) {
    // PostgREST or() ILIKE pattern. Échappe la virgule (séparateur d'expressions).
    const safe = q.replace(/[,()]/g, "");
    query = query.or(
      [
        `nom.ilike.%${safe}%`,
        `prenom.ilike.%${safe}%`,
        `email.ilike.%${safe}%`,
        `telephone.ilike.%${safe}%`,
        `code.ilike.%${safe}%`,
      ].join(","),
    );
  }

  const from = (page - 1) * perPage;
  const to = from + perPage - 1;

  query = query.order("nom", { ascending: true }).range(from, to);

  const { data, error, count } = await query;
  if (error) throw new Error(`listClients: ${error.message}`);

  const total = count ?? 0;
  return {
    rows: (data ?? []) as ClientRow[],
    total,
    page,
    perPage,
    pageCount: Math.max(1, Math.ceil(total / perPage)),
  };
}

export async function getClient(id: number): Promise<ClientRow | null> {
  const supabase = await createClient();
  const { data, error } = await supabase.from("clients").select("*").eq("id", id).maybeSingle();
  if (error) throw new Error(`getClient: ${error.message}`);
  return (data as ClientRow | null) ?? null;
}
