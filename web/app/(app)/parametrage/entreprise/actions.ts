"use server";

import { revalidatePath } from "next/cache";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireRole } from "@/lib/auth";
import { parseParametrageFormData } from "@/lib/schemas/parametrage";

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

/**
 * Upsert sur l'unique ligne `parametrages` (id = 1 si nouvelle, sinon update existant).
 */
export async function saveParametrageAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireRole([...adminRoles]);
  let input;
  try {
    input = parseParametrageFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    throw e;
  }
  const supabase = await createClient();
  const { data: existing } = await supabase
    .from("parametrages")
    .select("id")
    .order("id")
    .limit(1)
    .maybeSingle();
  if (existing?.id) {
    const { error } = await supabase.from("parametrages").update(input).eq("id", existing.id);
    if (error) return { error: error.message };
  } else {
    const { error } = await supabase.from("parametrages").insert(input);
    if (error) return { error: error.message };
  }
  revalidatePath("/parametrage");
  revalidatePath("/parametrage/entreprise");
  return { ok: true };
}
