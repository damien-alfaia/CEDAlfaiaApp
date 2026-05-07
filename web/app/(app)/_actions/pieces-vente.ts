"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import { z } from "zod";
import { createClient } from "@/lib/supabase/server";
import { requireProfile } from "@/lib/auth";
import {
  parsePieceVenteFormData,
  calculerTotaux,
  type PieceVenteWriteInput,
} from "@/lib/schemas/pieces-vente";
import { getNextNumDevis, getNextNumFacture } from "@/lib/data/pieces-vente";

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
// Save : create OR update une pièce de vente avec ses sous-entités.
//
// Logique :
//  - Si id == 0 : INSERT pieces_vente (avec num_devis ou num_facture auto).
//  - Sinon       : UPDATE pieces_vente (les num_* ne changent pas).
//  - Toujours    : DELETE puis INSERT en bloc des lignes/services/paiements.
//
// Les totaux sont calculés côté serveur à chaque sauvegarde.
// =============================================================================

async function savePieceVente(
  id: number,
  input: PieceVenteWriteInput,
): Promise<{ id: number; type: "devis" | "facture" }> {
  const supabase = await createClient();
  const totaux = calculerTotaux(input);

  const isFacture = input.type_piece === "facture";

  // Charge la pièce existante (pour conserver num_devis/num_facture et les flags)
  type Existing = {
    num_devis: number | null;
    num_facture: number | null;
    date_devis: string | null;
    date_facture: string | null;
    is_valide: boolean;
    is_facture_annule: boolean;
    is_envoye_comptable: boolean;
  };
  let existing: Existing | null = null;
  if (id > 0) {
    const { data, error } = await supabase
      .from("pieces_vente")
      .select(
        "num_devis, num_facture, date_devis, date_facture, is_valide, is_facture_annule, is_envoye_comptable",
      )
      .eq("id", id)
      .maybeSingle();
    if (error) throw new Error(error.message);
    existing = (data as Existing) ?? null;
  }

  // Numérotation
  let num_devis = existing?.num_devis ?? null;
  let num_facture = existing?.num_facture ?? null;
  if (!isFacture && num_devis === null) num_devis = await getNextNumDevis();
  if (isFacture && num_facture === null) num_facture = await getNextNumFacture();

  // Dates
  const date_devis = existing?.date_devis ?? input.date;
  const date_facture = isFacture ? (existing?.date_facture ?? input.date) : null;

  const row = {
    client_id: input.client_id,
    voiture_id: input.voiture_id,
    main_doeuvre_montant_horaire: input.main_doeuvre_montant_horaire,
    main_doeuvre_duree: input.main_doeuvre_duree,
    kilometrage: input.kilometrage,
    remise: input.remise,
    is_devis_envoye: input.is_devis_envoye,
    date_devis,
    num_devis,
    date_facture,
    num_facture,
    total_ht: totaux.total_ht,
    total_ttc: totaux.total_ttc,
    montant_tva: totaux.montant_tva,
    benefice_ttc: totaux.benefice_ttc,
    reste_a_payer: totaux.reste_a_payer,
  };

  let pieceId: number;
  if (id > 0) {
    const { error } = await supabase.from("pieces_vente").update(row).eq("id", id);
    if (error) throw new Error(error.message);
    pieceId = id;
  } else {
    const { data, error } = await supabase.from("pieces_vente").insert(row).select("id").single();
    if (error) throw new Error(error.message);
    pieceId = data!.id as number;
  }

  // Replace lignes/services/paiements (delete-then-insert).
  const { error: e1 } = await supabase
    .from("pieces_vente_lignes")
    .delete()
    .eq("piece_vente_id", pieceId);
  if (e1) throw new Error(e1.message);
  if (input.lignes.length > 0) {
    const { error } = await supabase.from("pieces_vente_lignes").insert(
      input.lignes.map((l) => ({
        piece_vente_id: pieceId,
        fournisseur_id: l.fournisseur_id ?? null,
        libelle: l.libelle,
        quantite: l.quantite,
        remise: l.remise,
        prix_garage_ht: l.prix_garage_ht,
        prix_garage_ttc: l.prix_garage_ttc,
        prix_client_ht: l.prix_client_ht,
        prix_client_ttc: l.prix_client_ttc,
      })),
    );
    if (error) throw new Error(error.message);
  }

  const { error: e2 } = await supabase
    .from("pieces_vente_services")
    .delete()
    .eq("piece_vente_id", pieceId);
  if (e2) throw new Error(e2.message);
  if (input.services.length > 0) {
    const { error } = await supabase.from("pieces_vente_services").insert(
      input.services.map((s) => ({
        piece_vente_id: pieceId,
        libelle: s.libelle,
        quantite: s.quantite,
        prix_client_ht: s.prix_client_ht,
        prix_client_ttc: s.prix_client_ttc,
      })),
    );
    if (error) throw new Error(error.message);
  }

  const { error: e3 } = await supabase
    .from("pieces_vente_paiements")
    .delete()
    .eq("piece_vente_id", pieceId);
  if (e3) throw new Error(e3.message);
  if (input.paiements.length > 0) {
    const { error } = await supabase.from("pieces_vente_paiements").insert(
      input.paiements.map((p) => ({
        piece_vente_id: pieceId,
        type_paiement: p.type_paiement,
        date: p.date,
        montant: p.montant,
      })),
    );
    if (error) throw new Error(error.message);
  }

  return { id: pieceId, type: isFacture ? "facture" : "devis" };
}

// =============================================================================
// CREATE
// =============================================================================

export async function createPieceVenteAction(
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input: PieceVenteWriteInput;
  try {
    input = parsePieceVenteFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    return { error: e instanceof Error ? e.message : "Erreur de parsing" };
  }
  let result;
  try {
    result = await savePieceVente(0, input);
  } catch (e) {
    return { error: e instanceof Error ? e.message : "Erreur" };
  }
  revalidatePath("/devis");
  revalidatePath("/factures");
  redirect(result.type === "facture" ? `/factures/${result.id}` : `/devis/${result.id}`);
}

// =============================================================================
// UPDATE
// =============================================================================

export async function updatePieceVenteAction(
  id: number,
  _prev: ActionState,
  form: FormData,
): Promise<ActionState> {
  await requireProfile();
  let input: PieceVenteWriteInput;
  try {
    input = parsePieceVenteFormData(form);
  } catch (e) {
    if (e instanceof z.ZodError) return fromZod(e);
    return { error: e instanceof Error ? e.message : "Erreur de parsing" };
  }
  try {
    await savePieceVente(id, input);
  } catch (e) {
    return { error: e instanceof Error ? e.message : "Erreur" };
  }
  revalidatePath("/devis");
  revalidatePath("/factures");
  revalidatePath(`/devis/${id}`);
  revalidatePath(`/factures/${id}`);
  return { ok: true };
}

// =============================================================================
// Transitions d'état
// =============================================================================

export async function genererFactureAction(devisId: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const numFacture = await getNextNumFacture();
  const { error } = await supabase
    .from("pieces_vente")
    .update({
      num_facture: numFacture,
      date_facture: new Date().toISOString(),
    })
    .eq("id", devisId)
    .is("date_facture", null);
  if (error) throw new Error(error.message);
  revalidatePath("/devis");
  revalidatePath("/factures");
  redirect(`/factures/${devisId}`);
}

export async function validerFactureAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("pieces_vente")
    .update({
      is_valide: true,
      date_heure_validation: new Date().toISOString(),
    })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/factures");
  revalidatePath(`/factures/${id}`);
}

export async function annulerFactureAction(id: number, commentaire: string): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("pieces_vente")
    .update({
      is_facture_annule: true,
      date_heure_annulation: new Date().toISOString(),
      commentaire_annulation: commentaire || null,
    })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/factures");
  revalidatePath(`/factures/${id}`);
}

export async function repasserFactureAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("pieces_vente")
    .update({
      is_facture_annule: false,
      date_heure_annulation: null,
      commentaire_annulation: null,
    })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/factures");
  revalidatePath(`/factures/${id}`);
}

export async function envoyerComptableAction(ids: number[]): Promise<void> {
  await requireProfile();
  if (ids.length === 0) return;
  const supabase = await createClient();
  const { error } = await supabase
    .from("pieces_vente")
    .update({ is_envoye_comptable: true })
    .in("id", ids);
  if (error) throw new Error(error.message);
  revalidatePath("/factures");
}

export async function deleteDevisAction(id: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  // Permis uniquement si c'est encore un devis (pas de facture)
  const { error } = await supabase
    .from("pieces_vente")
    .delete()
    .eq("id", id)
    .is("date_facture", null);
  if (error) throw new Error(error.message);
  revalidatePath("/devis");
  redirect("/devis");
}

export async function setDevisEnvoyeAction(id: number, value: boolean): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { error } = await supabase
    .from("pieces_vente")
    .update({ is_devis_envoye: value })
    .eq("id", id);
  if (error) throw new Error(error.message);
  revalidatePath("/devis");
  revalidatePath(`/devis/${id}`);
}
