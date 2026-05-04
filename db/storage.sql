-- =============================================================================
-- CEDAlfaiaApp v3 — Buckets Supabase Storage
-- =============================================================================
-- À exécuter APRÈS schema.sql.
-- Crée les buckets pour stocker :
--   - les documents joints aux devis/factures (devis fournisseur, BL, etc.)
--   - le logo et l'entête configurés dans Parametrage
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Bucket : documents (privé)
--   Contenu : tous les byte[] de la table documents (ex-DocumentPieceVente)
--   Convention de chemin : pieces-vente/{piece_vente_id}/{document_id}
--   Accès : authentifié uniquement (policies dans rls.sql)
-- -----------------------------------------------------------------------------

insert into storage.buckets (id, name, public, file_size_limit, allowed_mime_types)
values (
  'documents',
  'documents',
  false,
  10485760, -- 10 MiB par fichier
  array[
    'application/pdf',
    'image/png',
    'image/jpeg',
    'image/jpg',
    'image/webp',
    'image/heic'
  ]
)
on conflict (id) do nothing;

-- -----------------------------------------------------------------------------
-- Bucket : parametrage (privé)
--   Contenu : Parametrage.Logo et Parametrage.Entete
--   Convention de chemin :
--     - logo/{parametrage_id}
--     - entete/{parametrage_id}
--   Accès : authentifié uniquement (policies dans rls.sql)
-- -----------------------------------------------------------------------------

insert into storage.buckets (id, name, public, file_size_limit, allowed_mime_types)
values (
  'parametrage',
  'parametrage',
  false,
  5242880, -- 5 MiB
  array[
    'image/png',
    'image/jpeg',
    'image/jpg',
    'image/webp',
    'image/svg+xml'
  ]
)
on conflict (id) do nothing;
