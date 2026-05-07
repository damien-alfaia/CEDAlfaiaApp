"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireRole } from "@/lib/auth";
import { parseSalarieFormData } from "@/lib/schemas/salaries";

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

export async function createSalarieAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseSalarieFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { data, error } = await supabase.from("salaries").insert(input).select("id").single();
  if (error) return { error: error.message };
  revalidatePath("/salaries");
  redirect(`/salaries/${data.id}`);
}

export async function updateSalarieAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseSalarieFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { error } = await supabase.from("salaries").update(input).eq("id", id);
  if (error) return { error: error.message };
  revalidatePath("/salaries");
  revalidatePath(`/salaries/${id}`);
  return { ok: true };
}

export async function deleteSalarieAction(id: number): Promise<void> {
  await requireRole([...adminRoles]);
  const supabase = await createClient();
  const { error } = await supabase.from("salaries").delete().eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/salaries");
  redirect("/salaries");
}
