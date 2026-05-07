"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireProfile } from "@/lib/auth";
import { parseClientFormData } from "@/lib/schemas/clients";
import { parseVoitureFormData } from "@/lib/schemas/voitures";

export type ActionState = { ok?: boolean; error?: string; fieldErrors?: Record<string, string> };

function fromZod(err: z.ZodError): ActionState {
  const fieldErrors: Record<string, string> = {};
  for (const issue of err.issues) {
    const key = issue.path.join(".");
    if (!fieldErrors[key]) fieldErrors[key] = issue.message;
  }
  return { error: "Données invalides", fieldErrors };
}

// =============================================================================
// Clients
// =============================================================================

export async function createClientAction(_prev: ActionState, form: FormData): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseClientFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }

  const supabase = await createClient();
  const { data, error } = await supabase.from("clients").insert(input).select("id").single();
  if (error) return { error: error.message };

  revalidatePath("/clients");
  redirect(`/clients/${data.id}`);
}

export async function updateClientAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseClientFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }

  const supabase = await createClient();
  const { error } = await supabase.from("clients").update(input).eq("id", id);
  if (error) return { error: error.message };

  revalidatePath("/clients");
  revalidatePath(`/clients/${id}`);
  return { ok: true };
}

/**
 * Soft delete : positionne date_suppression à now().
 * Cohérent avec l'app actuelle (filtrage où date_suppression IS NULL).
 */
export async function softDeleteClientAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("clients")
    .update({ date_suppression: new Date().toISOString() })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/clients");
  redirect("/clients");
}

export async function restoreClientAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase.from("clients").update({ date_suppression: null }).eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/clients");
  revalidatePath(`/clients/${id}`);
}

// =============================================================================
// Voitures (rattachées à un client)
// =============================================================================

async function ensureSingleVoiturePrincipale(
  clientId: number,
  exceptVoitureId: number | null,
): Promise<void> {
  const supabase = await createClient();
  let q = supabase
    .from("voitures")
    .update({ is_principale: false })
    .eq("client_id", clientId)
    .eq("is_principale", true);
  if (exceptVoitureId !== null) {
    q = q.neq("id", exceptVoitureId);
  }
  const { error } = await q;
  if (error) throw new Error(`ensureSingleVoiturePrincipale: ${error.message}`);
}

export async function createVoitureAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseVoitureFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }

  const supabase = await createClient();
  if (input.is_principale) {
    await ensureSingleVoiturePrincipale(input.client_id, null);
  }
  const { error } = await supabase.from("voitures").insert(input);
  if (error) return { error: error.message };

  revalidatePath(`/clients/${input.client_id}`);
  return { ok: true };
}

export async function updateVoitureAction(
  voitureId: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseVoitureFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }

  const supabase = await createClient();
  if (input.is_principale) {
    await ensureSingleVoiturePrincipale(input.client_id, voitureId);
  }
  const { error } = await supabase
    .from("voitures")
    .update({
      immatriculation: input.immatriculation,
      modele_id: input.modele_id,
      is_principale: input.is_principale,
    })
    .eq("id", voitureId);
  if (error) return { error: error.message };

  revalidatePath(`/clients/${input.client_id}`);
  return { ok: true };
}

export async function softDeleteVoitureAction(voitureId: number, clientId: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("voitures")
    .update({ date_suppression: new Date().toISOString() })
    .eq("id", voitureId);
  if (error) throw new Error(error.message);
  revalidatePath(`/clients/${clientId}`);
}
