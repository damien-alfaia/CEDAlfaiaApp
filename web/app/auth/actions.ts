"use server";

import { createClient } from "@/lib/supabase/server";
import { redirect } from "next/navigation";
import { z } from "zod";

const loginSchema = z.object({
  email: z.string().email("Email invalide"),
  password: z.string().min(1, "Mot de passe requis"),
});

export type LoginState = {
  error?: string;
};

export async function signIn(_prev: LoginState, formData: FormData): Promise<LoginState> {
  const parsed = loginSchema.safeParse({
    email: formData.get("email"),
    password: formData.get("password"),
  });

  if (!parsed.success) {
    return { error: parsed.error.issues[0]?.message ?? "Données invalides" };
  }

  const supabase = await createClient();
  const { error } = await supabase.auth.signInWithPassword(parsed.data);

  if (error) {
    return { error: "Email ou mot de passe incorrect." };
  }

  redirect("/");
}

export async function signOut() {
  const supabase = await createClient();
  await supabase.auth.signOut();
  redirect("/login");
}

// =============================================================================
// Inscription protégée par code
// =============================================================================

const signUpSchema = z
  .object({
    email: z.string().trim().email("Email invalide"),
    password: z
      .string()
      .min(8, "Mot de passe trop court (8 caractères minimum)")
      .max(72, "Mot de passe trop long"),
    confirmPassword: z.string(),
    code: z.string().min(1, "Code d'inscription requis"),
  })
  .refine((d) => d.password === d.confirmPassword, {
    message: "Les mots de passe ne correspondent pas",
    path: ["confirmPassword"],
  });

export type SignUpState = {
  error?: string;
  fieldErrors?: Record<string, string>;
  success?: boolean;
  needsConfirmation?: boolean;
};

export async function signUp(_prev: SignUpState, formData: FormData): Promise<SignUpState> {
  const parsed = signUpSchema.safeParse({
    email: formData.get("email"),
    password: formData.get("password"),
    confirmPassword: formData.get("confirmPassword"),
    code: formData.get("code"),
  });
  if (!parsed.success) {
    const fieldErrors: Record<string, string> = {};
    for (const issue of parsed.error.issues) {
      const k = issue.path.join(".");
      if (!fieldErrors[k]) fieldErrors[k] = issue.message;
    }
    return { error: "Données invalides", fieldErrors };
  }

  // Code d'inscription : variable d'env serveur, jamais exposée au client.
  const expected = process.env.SIGNUP_CODE;
  if (!expected) {
    return {
      error: "L'inscription n'est pas activée sur ce serveur (SIGNUP_CODE non configuré).",
    };
  }
  if (parsed.data.code !== expected) {
    return {
      error: "Code d'inscription invalide.",
      fieldErrors: { code: "Code invalide" },
    };
  }

  const supabase = await createClient();
  const { data, error } = await supabase.auth.signUp({
    email: parsed.data.email,
    password: parsed.data.password,
  });
  if (error) {
    // Cas typique : email déjà inscrit / mot de passe faible côté Supabase
    return { error: error.message };
  }

  // Si la confirmation email est activée côté Supabase, `data.session` est null
  // tant que l'email n'a pas été cliqué. On informe l'utilisateur.
  const needsConfirmation = !data.session;
  return { success: true, needsConfirmation };
}
