"use server";

import { revalidatePath } from "next/cache";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireRole } from "@/lib/auth";
import { parseMarqueFormData, parseModeleFormData } from "@/lib/schemas/referentiels";

export type ActionState = { ok?: boolean; error?: string; fieldErrors?: Record<string, string> };

function fromZod(err: z.ZodError): ActionState {
  const fieldErrors: Record<string, string> = {};
  for (const issue of err.issues) {
    const key = issue.path.join(".");
    if (!fieldErrors[key]) fieldErrors[key] = issue.message;
  }
  return { error: "Données invalides", fieldErrors };
}

const adminRoles = ["super_admin", "administrateur"] as const;

// =============================================================================
// Marques
// =============================================================================

export async function createMarqueAction(_prev: ActionState, form: FormData): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseMarqueFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("marques").insert(input);
  if (error) return { error: error.message };
  revalidatePath("/parametrage/referentiels");
  return { ok: true };
}

export async function updateMarqueAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseMarqueFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("marques").update(input).eq("id", id);
  if (error) return { error: error.message };
  revalidatePath("/parametrage/referentiels");
  return { ok: true };
}

export async function deleteMarqueAction(id: number): Promise<void> {
  await requireRole([...adminRoles]);
  const supabase = await createClient();
  const { error } = await supabase.from("marques").delete().eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/parametrage/referentiels");
}

// =============================================================================
// Modèles
// =============================================================================

export async function createModeleAction(_prev: ActionState, form: FormData): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseModeleFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("modeles").insert(input);
  if (error) return { error: error.message };
  revalidatePath("/parametrage/referentiels");
  return { ok: true };
}

export async function updateModeleAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseModeleFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase
    .from("modeles")
    .update({ libelle: input.libelle, marque_id: input.marque_id })
    .eq("id", id);
  if (error) return { error: error.message };
  revalidatePath("/parametrage/referentiels");
  return { ok: true };
}

export async function deleteModeleAction(id: number): Promise<void> {
  await requireRole([...adminRoles]);
  const supabase = await createClient();
  const { error } = await supabase.from("modeles").delete().eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/parametrage/referentiels");
}
