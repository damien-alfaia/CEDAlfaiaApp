"use server";

import { revalidatePath } from "next/cache";
import { createClient } from "@/lib/supabase/server";
import { requireProfile } from "@/lib/auth";
import type { TypeDocument } from "@/lib/db/types";

export type UploadState = { ok?: boolean; error?: string };

const MAX_BYTES = 10 * 1024 * 1024; // 10 MiB (cohérent avec storage.sql)
const ALLOWED_TYPES: TypeDocument[] = [
  "devis_fournisseur",
  "bon_livraison_fournisseur",
  "devis",
  "facture",
];

/**
 * Upload un fichier dans le bucket `documents` puis insère une ligne
 * dans la table documents pointant vers ce fichier.
 *
 * FormData attendue : file (File), piece_vente_id (number), type_document (string).
 */
export async function uploadDocumentAction(
  _prev: UploadState,
  form: FormData,
): Promise<UploadState> {
  await requireProfile();

  const file = form.get("file");
  const pieceVenteIdRaw = form.get("piece_vente_id");
  const typeRaw = form.get("type_document");

  if (!(file instanceof File) || file.size === 0) {
    return { error: "Sélectionnez un fichier." };
  }
  if (file.size > MAX_BYTES) {
    return { error: `Fichier trop volumineux (max ${Math.round(MAX_BYTES / 1024 / 1024)} MiB).` };
  }
  const pieceVenteId = Number(pieceVenteIdRaw);
  if (!Number.isInteger(pieceVenteId) || pieceVenteId <= 0) {
    return { error: "ID pièce de vente invalide." };
  }
  const typeDocument = (typeRaw?.toString() ?? "") as TypeDocument;
  if (!ALLOWED_TYPES.includes(typeDocument)) {
    return { error: "Type de document invalide." };
  }

  const supabase = await createClient();

  const ext = file.name.includes(".") ? "." + file.name.split(".").pop() : "";
  // Nom unique pour éviter les collisions ; le piece_vente_id structure le chemin.
  const path = `pieces-vente/${pieceVenteId}/${crypto.randomUUID()}${ext}`;

  const { error: upErr } = await supabase.storage
    .from("documents")
    .upload(path, file, { contentType: file.type || "application/octet-stream" });
  if (upErr) {
    return { error: `Upload : ${upErr.message}` };
  }

  const { error: insErr } = await supabase.from("documents").insert({
    piece_vente_id: pieceVenteId,
    libelle: file.name,
    type_document: typeDocument,
    storage_path: path,
    format_fichier: file.type || null,
    date_creation: new Date().toISOString(),
    date_modification: new Date().toISOString(),
  });
  if (insErr) {
    // Cleanup le fichier uploadé pour rester cohérent.
    await supabase.storage.from("documents").remove([path]);
    return { error: `Insertion : ${insErr.message}` };
  }

  revalidatePath(`/devis/${pieceVenteId}`);
  revalidatePath(`/factures/${pieceVenteId}`);
  return { ok: true };
}

export async function deleteDocumentAction(documentId: number): Promise<void> {
  await requireProfile();
  const supabase = await createClient();
  const { data: doc, error: getErr } = await supabase
    .from("documents")
    .select("id, piece_vente_id, storage_path")
    .eq("id", documentId)
    .maybeSingle();
  if (getErr) throw new Error(getErr.message);
  if (!doc) return;
  if (doc.storage_path) {
    await supabase.storage.from("documents").remove([doc.storage_path]);
  }
  const { error: delErr } = await supabase.from("documents").delete().eq("id", documentId);
  if (delErr) throw new Error(delErr.message);
  revalidatePath(`/devis/${doc.piece_vente_id}`);
  revalidatePath(`/factures/${doc.piece_vente_id}`);
}
