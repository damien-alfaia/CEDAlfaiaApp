import { createClient } from "@/lib/supabase/server";
import { redirect } from "next/navigation";

export type UserRole = "super_admin" | "administrateur" | "secretaire" | "technicien";

export type Profile = {
  id: string;
  email: string;
  prenom: string | null;
  nom: string | null;
  role: UserRole;
  entreprise_id: number | null;
};

/**
 * Returns the current authenticated user + their profile, or null if not signed in.
 * Use this in Server Components / Route Handlers to check auth state.
 */
export async function getCurrentProfile(): Promise<Profile | null> {
  const supabase = await createClient();
  const {
    data: { user },
  } = await supabase.auth.getUser();

  if (!user) return null;

  const { data, error } = await supabase
    .from("profiles")
    .select("id, email, prenom, nom, role, entreprise_id")
    .eq("id", user.id)
    .single();

  if (error || !data) {
    // Auth user without profile row — should not happen thanks to the trigger,
    // but bail safely.
    return null;
  }

  return data as Profile;
}

/**
 * Helper for protected pages : redirects to /login if not signed in.
 * Returns the profile so the caller can use it (display name, role gating).
 */
export async function requireProfile(): Promise<Profile> {
  const profile = await getCurrentProfile();
  if (!profile) redirect("/login");
  return profile;
}

/**
 * Helper for role-protected pages : redirects to /403 if role is not allowed.
 */
export async function requireRole(allowed: UserRole[]): Promise<Profile> {
  const profile = await requireProfile();
  if (!allowed.includes(profile.role)) redirect("/403");
  return profile;
}

/**
 * Display name for the topbar / breadcrumbs.
 */
export function displayName(profile: Profile): string {
  const full = [profile.prenom, profile.nom].filter(Boolean).join(" ").trim();
  return full || profile.email;
}

/**
 * Initials for avatar fallback.
 */
export function initials(profile: Profile): string {
  const full = [profile.prenom, profile.nom].filter(Boolean).join(" ").trim();
  if (!full) return profile.email.slice(0, 2).toUpperCase();
  return full
    .split(/\s+/)
    .map((p) => p[0]?.toUpperCase() ?? "")
    .slice(0, 2)
    .join("");
}
