"use server";

import { revalidatePath } from "next/cache";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireProfile } from "@/lib/auth";
import { parseRendezVousFormData } from "@/lib/schemas/agenda";

export type ActionState = { ok?: boolean; error?: string; fieldErrors?: Record<string, string> };

function fromZod(err: z.ZodError): ActionState {
  const fieldErrors: Record<string, string> = {};
  for (const issue of err.issues) {
    const key = issue.path.join(".");
    if (!fieldErrors[key]) fieldErrors[key] = issue.message;
  }
  return { error: "Données invalides", fieldErrors };
}

export async function createRendezVousAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseRendezVousFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("rendez_vous").insert(input);
  if (error) return { error: error.message };
  revalidatePath("/agenda");
  return { ok: true };
}

export async function updateRendezVousAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input;
  try {
    input = parseRendezVousFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("rendez_vous").update(input).eq("id", id);
  if (error) return { error: error.message };
  revalidatePath("/agenda");
  return { ok: true };
}

export async function deleteRendezVousAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase.from("rendez_vous").delete().eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/agenda");
}
