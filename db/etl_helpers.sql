-- =============================================================================
-- ETL helpers — fonctions Postgres appelées par etl/migrate.ts
-- =============================================================================
-- À exécuter UNE FOIS sur chaque projet Supabase (staging puis prod) avant
-- la première migration de données. Les fonctions sont en SECURITY DEFINER
-- pour pouvoir bypass RLS quand appelées via la service_role key.
-- =============================================================================

-- Reset toutes les sequences id à MAX(id) après une insertion explicite.
-- À appeler via supabase.rpc('etl_reset_all_sequences') juste après l'ETL.
create or replace function etl_reset_all_sequences()
returns void
language plpgsql
security definer
set search_path = public
as $$
declare
  rec record;
begin
  for rec in
    select c.relname as seq, t.relname as tbl
    from pg_class c
    join pg_depend d on d.objid = c.oid
    join pg_class t on t.oid = d.refobjid
    join pg_attribute a on a.attrelid = t.oid and a.attnum = d.refobjsubid
    where c.relkind = 'S'
      and t.relkind = 'r'
      and t.relnamespace = 'public'::regnamespace
      and a.attname = 'id'
  loop
    execute format(
      'select setval(%L, coalesce((select max(id) from public.%I), 1), true)',
      'public.' || rec.seq, rec.tbl
    );
  end loop;
end $$;

-- Vide toutes les tables de données métier sans toucher à profiles ni à
-- auth.users.
--
-- Pourquoi pas TRUNCATE CASCADE : `profiles` a une FK vers `entreprises`
-- (entreprise_id), donc TRUNCATE CASCADE sur entreprises wipe profiles
-- et casse l'auth des utilisateurs déjà inscrits (incident vécu lors du
-- 1er ETL prod). On utilise DELETE chirurgical à la place.
-- Ordre inverse des FK pour respecter les contraintes sans CASCADE.
create or replace function etl_truncate_all_data()
returns void
language plpgsql
security definer
set search_path = public
as $$
begin
  delete from pieces_vente_paiements;
  delete from pieces_vente_services;
  delete from pieces_vente_lignes;
  delete from documents;
  delete from pieces_vente;
  delete from voitures;
  delete from clients;
  delete from fournisseurs;
  delete from rendez_vous;
  delete from modeles;
  delete from marques;
  delete from salarie_indisponibilites;
  delete from salarie_salaires;
  delete from salarie_contrats;
  delete from salaries;
  delete from widgets_utilisateurs;
  delete from widgets;
  -- Détacher profiles.entreprise_id avant de vider entreprises pour
  -- préserver les comptes utilisateurs déjà inscrits.
  update profiles set entreprise_id = null where entreprise_id is not null;
  delete from entreprises;
  delete from parametrages;
end $$;

-- Verrouille les fonctions pour qu'elles ne soient pas appelables par
-- l'utilisateur anon (uniquement service_role).
revoke all on function etl_reset_all_sequences() from public, anon, authenticated;
revoke all on function etl_truncate_all_data()  from public, anon, authenticated;
grant execute on function etl_reset_all_sequences() to service_role;
grant execute on function etl_truncate_all_data()  to service_role;
