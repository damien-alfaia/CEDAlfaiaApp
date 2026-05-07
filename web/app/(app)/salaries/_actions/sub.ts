"use server";

import { revalidatePath } from "next/cache";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireRole } from "@/lib/auth";
import {
  parseContratFormData,
  parseIndisponibiliteFormData,
  parseSalaireFormData,
} from "@/lib/schemas/salaries-sub";

export type ActionState = { ok?: boolean; error?: string; fieldErrors?: Record<string, string> };

const adminRoles = ["super_admin", "administrateur"] as const;

function fromZod(err: z.ZodError): ActionState {
  const fieldErrors: Record<string, string> = {};
  for (const issue of err.issues) {
    const key = issue.path.join(".");
    if (!fieldErrors[key]) fieldErrors[key] = issue.message;
  }
  return { error: "Données invalides", fieldErrors };
}

// =============================================================================
// Contrats
// =============================================================================

export async function createContratAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseContratFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_contrats").insert(input);
  if (error) return { error: error.message };
  revalidatePath(`/salaries/${input.salarie_id}`);
  return { ok: true };
}

export async function updateContratAction(
  contratId: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseContratFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_contrats").update(input).eq("id", contratId);
  if (error) return { error: error.message };
  revalidatePath(`/salaries/${input.salarie_id}`);
  revalidatePath(`/salaries/${input.salarie_id}/contrats/${contratId}`);
  return { ok: true };
}

export async function deleteContratAction(contratId: number, salarieId: number): Promise<void> {
  await requireRole([...adminRoles]);
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_contrats").delete().eq("id", contratId);
  if (error) throw new Error(error.message);
  revalidatePath(`/salaries/${salarieId}`);
}

// =============================================================================
// Indisponibilités
// =============================================================================

export async function createIndisponibiliteAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseIndisponibiliteFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_indisponibilites").insert(input);
  if (error) return { error: error.message };
  revalidatePath(`/salaries`);
  return { ok: true };
}

export async function deleteIndisponibiliteAction(id: number): Promise<void> {
  await requireRole([...adminRoles]);
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_indisponibilites").delete().eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/salaries");
}

// =============================================================================
// Bulletins
// =============================================================================

export async function createSalaireAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseSalaireFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_salaires").insert(input);
  if (error) return { error: error.message };
  revalidatePath("/salaries");
  return { ok: true };
}

export async function deleteSalaireAction(id: number): Promise<void> {
  await requireRole([...adminRoles]);
  const supabase = await createClient();
  const { error } = await supabase.from("salarie_salaires").delete().eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/salaries");
}
