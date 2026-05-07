"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireProfile } from "@/lib/auth";
import { parseFournisseurFormData } from "@/lib/schemas/fournisseurs";

export type ActionState = { ok?: boolean; error?: string; fieldErrors?: Record<string, string> };

function fromZod(err: z.ZodError): ActionState {
  const fieldErrors: Record<string, string> = {};
  for (const issue of err.issues) {
    const key = issue.path.join(".");
    if (!fieldErrors[key]) fieldErrors[key] = issue.message;
  }
  return { error: "Données invalides", fieldErrors };
}

export async function createFournisseurAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseFournisseurFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { data, error } = await supabase.from("fournisseurs").insert(input).select("id").single();
  if (error) return { error: error.message };
  revalidatePath("/fournisseurs");
  redirect(`/fournisseurs/${data.id}`);
}

export async function updateFournisseurAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseFournisseurFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("fournisseurs").update(input).eq("id", id);
  if (error) return { error: error.message };
  revalidatePath("/fournisseurs");
  revalidatePath(`/fournisseurs/${id}`);
  return { ok: true };
}

export async function softDeleteFournisseurAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("fournisseurs")
    .update({ date_suppression: new Date().toISOString() })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/fournisseurs");
  redirect("/fournisseurs");
}

export async function restoreFournisseurAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("fournisseurs")
    .update({ date_suppression: null })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/fournisseurs");
  revalidatePath(`/fournisseurs/${id}`);
}
