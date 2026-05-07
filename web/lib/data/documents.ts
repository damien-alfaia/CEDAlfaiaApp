import "server-only";
import { createClient } from "@/lib/supabase/server";
import type { TypeDocument } from "@/lib/db/types";

export type DocumentRow = {
  id: number;
  piece_vente_id: number;
  libelle: string | null;
  type_document: TypeDocument;
  storage_path: string | null;
  format_fichier: string | null;
  date_creation: string | null;
};

export async function listDocumentsForPiece(pieceVenteId: number): Promise<DocumentRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("documents")
    .select(
      "id, piece_vente_id, libelle, type_document, storage_path, format_fichier, date_creation",
    )
    .eq("piece_vente_id", pieceVenteId)
    .order("date_creation", { ascending: false });
  if (error) throw new Error(`listDocumentsForPiece: ${error.message}`);
  return (data ?? []) as DocumentRow[];
}

/**
 * Crée une URL signée temporaire pour télécharger un fichier privé.
 * Durée par défaut : 5 min.
 */
export async function getSignedDownloadUrl(
  storagePath: string,
  expiresIn: number = 300,
): Promise<string | null> {
  const supabase = await createClient();
  const { data, error } = await supabase.storage
    .from("documents")
    .createSignedUrl(storagePath, expiresIn);
  if (error) {
    console.error(`createSignedUrl(${storagePath}): ${error.message}`);
    return null;
  }
  return data.signedUrl;
}
