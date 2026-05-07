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

-- Truncate toutes les tables de données métier (sans toucher à profiles
-- ou auth.users). Utilise CASCADE+RESTART IDENTITY pour repartir propre.
-- À appeler avant l'ETL pour garantir l'idempotence.
create or replace function etl_truncate_all_data()
returns void
language plpgsql
security definer
set search_path = public
as $$
begin
  -- Ordre inverse des FK (juste pour la lisibilité ; CASCADE s'en charge).
  truncate table
    pieces_vente_paiements,
    pieces_vente_services,
    pieces_vente_lignes,
    documents,
    pieces_vente,
    voitures,
    clients,
    fournisseurs,
    rendez_vous,
    modeles,
    marques,
    salarie_indisponibilites,
    salarie_salaires,
    salarie_contrats,
    salaries,
    widgets_utilisateurs,
    widgets,
    entreprises,
    parametrages
  restart identity cascade;
end $$;

-- Verrouille les fonctions pour qu'elles ne soient pas appelables par
-- l'utilisateur anon (uniquement service_role).
revoke all on function etl_reset_all_sequences() from public, anon, authenticated;
revoke all on function etl_truncate_all_data()  from public, anon, authenticated;
grant execute on function etl_reset_all_sequences() to service_role;
grant execute on function etl_truncate_all_data()  to service_role;
