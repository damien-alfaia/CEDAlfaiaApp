-- =============================================================================
-- CEDAlfaiaApp v3 — Politiques RLS (Row Level Security)
-- =============================================================================
-- À exécuter APRÈS schema.sql et storage.sql.
--
-- Modèle d'autorisation (mono-tenant, un seul utilisateur prévu) :
--   "Toute opération CRUD est autorisée si auth.uid() existe dans profiles."
--
-- Les rôles user_role (super_admin, administrateur, ...) servent uniquement
-- côté Next.js pour adapter l'UI ; ils ne segmentent PAS les permissions DB
-- pour l'instant. À durcir si un second utilisateur arrive.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Helper : vérifier que l'utilisateur courant a un profil
-- -----------------------------------------------------------------------------

create or replace function is_authenticated_profile()
returns boolean
language sql
security definer
set search_path = public
stable
as $$
  select exists (
    select 1 from public.profiles where id = auth.uid()
  );
$$;

-- -----------------------------------------------------------------------------
-- Activation RLS sur toutes les tables métier
-- -----------------------------------------------------------------------------

alter table parametrages              enable row level security;
alter table entreprises               enable row level security;
alter table profiles                  enable row level security;
alter table marques                   enable row level security;
alter table modeles                   enable row level security;
alter table clients                   enable row level security;
alter table voitures                  enable row level security;
alter table fournisseurs              enable row level security;
alter table rendez_vous               enable row level security;
alter table pieces_vente              enable row level security;
alter table pieces_vente_lignes       enable row level security;
alter table pieces_vente_services     enable row level security;
alter table pieces_vente_paiements    enable row level security;
alter table documents                 enable row level security;
alter table salaries                  enable row level security;
alter table salarie_contrats          enable row level security;
alter table salarie_indisponibilites  enable row level security;
alter table salarie_salaires          enable row level security;
alter table widgets                   enable row level security;
alter table widgets_utilisateurs      enable row level security;

-- -----------------------------------------------------------------------------
-- Policies génériques "authentifié = full access"
-- -----------------------------------------------------------------------------

create policy auth_full_access_parametrages           on parametrages           for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_entreprises            on entreprises            for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_marques                on marques                for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_modeles                on modeles                for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_clients                on clients                for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_voitures               on voitures               for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_fournisseurs           on fournisseurs           for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_rendez_vous            on rendez_vous            for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_pieces_vente           on pieces_vente           for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_pieces_vente_lignes    on pieces_vente_lignes    for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_pieces_vente_services  on pieces_vente_services  for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_pieces_vente_paiements on pieces_vente_paiements for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_documents              on documents              for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_salaries               on salaries               for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_salarie_contrats       on salarie_contrats       for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_salarie_indisponibilites on salarie_indisponibilites for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_salarie_salaires       on salarie_salaires       for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());
create policy auth_full_access_widgets                on widgets                for all to authenticated using (is_authenticated_profile()) with check (is_authenticated_profile());

-- -----------------------------------------------------------------------------
-- Policies spéciales : profiles
--   - Un utilisateur peut lire / mettre à jour SON propre profil.
--   - Création (signup) : l'insertion se fait via trigger sur auth.users,
--     pas via l'utilisateur lui-même.
--   - Pas de delete via API (uniquement via cascade depuis auth.users).
-- -----------------------------------------------------------------------------

create policy profiles_select_self
  on profiles
  for select
  to authenticated
  using (id = auth.uid() or is_authenticated_profile());

create policy profiles_update_self
  on profiles
  for update
  to authenticated
  using (id = auth.uid())
  with check (id = auth.uid());

-- -----------------------------------------------------------------------------
-- Policies spéciales : widgets_utilisateurs
--   Chaque utilisateur ne voit / ne modifie QUE ses propres widgets.
-- -----------------------------------------------------------------------------

create policy widgets_utilisateurs_owner
  on widgets_utilisateurs
  for all
  to authenticated
  using (profile_id = auth.uid())
  with check (profile_id = auth.uid());

-- -----------------------------------------------------------------------------
-- Trigger : création automatique du profil à la création d'un auth.user
--   Permet à un nouvel utilisateur Supabase Auth d'avoir son entrée
--   profiles auto-générée. À ajuster manuellement (rôle, entreprise) après.
-- -----------------------------------------------------------------------------

create or replace function handle_new_auth_user()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
begin
  insert into public.profiles (id, email, role)
  values (new.id, new.email, 'administrateur')
  on conflict (id) do nothing;
  return new;
end;
$$;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
  after insert on auth.users
  for each row execute function handle_new_auth_user();

-- -----------------------------------------------------------------------------
-- Storage policies : bucket "documents"
-- -----------------------------------------------------------------------------

create policy storage_documents_select
  on storage.objects for select
  to authenticated
  using (bucket_id = 'documents' and is_authenticated_profile());

create policy storage_documents_insert
  on storage.objects for insert
  to authenticated
  with check (bucket_id = 'documents' and is_authenticated_profile());

create policy storage_documents_update
  on storage.objects for update
  to authenticated
  using (bucket_id = 'documents' and is_authenticated_profile())
  with check (bucket_id = 'documents' and is_authenticated_profile());

create policy storage_documents_delete
  on storage.objects for delete
  to authenticated
  using (bucket_id = 'documents' and is_authenticated_profile());

-- -----------------------------------------------------------------------------
-- Storage policies : bucket "parametrage"
-- -----------------------------------------------------------------------------

create policy storage_parametrage_select
  on storage.objects for select
  to authenticated
  using (bucket_id = 'parametrage' and is_authenticated_profile());

create policy storage_parametrage_insert
  on storage.objects for insert
  to authenticated
  with check (bucket_id = 'parametrage' and is_authenticated_profile());

create policy storage_parametrage_update
  on storage.objects for update
  to authenticated
  using (bucket_id = 'parametrage' and is_authenticated_profile())
  with check (bucket_id = 'parametrage' and is_authenticated_profile());

create policy storage_parametrage_delete
  on storage.objects for delete
  to authenticated
  using (bucket_id = 'parametrage' and is_authenticated_profile());
