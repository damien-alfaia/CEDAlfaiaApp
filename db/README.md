# Schéma Supabase — CEDAlfaiaApp v3

Livrable de la **Phase 1** du plan de modernisation.

## Contenu

| Fichier | Rôle |
|---|---|
| `MAPPING.md` | Documentation des choix de mapping SQL Server → Postgres |
| `schema.sql` | Définition complète : enums, tables, indexes, triggers `updated_at` |
| `storage.sql` | Création des buckets Supabase Storage (`documents`, `parametrage`) |
| `rls.sql` | Activation RLS, policies tables, policies storage, trigger d'auto-création de `profiles` |

## Ordre d'exécution

```bash
psql "$SUPABASE_DB_URL" -f schema.sql
psql "$SUPABASE_DB_URL" -f storage.sql
psql "$SUPABASE_DB_URL" -f rls.sql
```

Ou via l'éditeur SQL du dashboard Supabase : exécuter les trois fichiers dans cet ordre, dans un projet **vide**.

## Vérification post-exécution

```sql
-- 20 tables métier attendues
select count(*) from information_schema.tables
  where table_schema = 'public' and table_type = 'BASE TABLE';
-- → 20

-- 4 enums attendus
select typname from pg_type
  where typtype = 'e' and typnamespace = 'public'::regnamespace
  order by typname;
-- → type_document, type_indisponibilite, type_paiement, user_role

-- 2 buckets Storage
select id from storage.buckets order by id;
-- → documents, parametrage

-- RLS activée sur toutes les tables métier
select tablename from pg_tables
  where schemaname = 'public' and rowsecurity = false;
-- → (vide)
```

## Création d'un utilisateur de test (après application)

Depuis le dashboard Supabase → Authentication → Add user → entrer email + mot de passe initial.

Le trigger `on_auth_user_created` créera automatiquement la ligne `profiles` correspondante avec `role = 'administrateur'`. Mettre à jour le profil ensuite (entreprise, prénom, nom) via SQL ou interface.

## Hors scope de Phase 1

- Données : aucune insertion ; le schéma est vide.
- Migration des données SQL Server → Postgres : c'est la **Phase 7**.
- Connexion depuis l'app Next.js : c'est la **Phase 2** (setup) puis **Phase 3** (auth).

## Limites connues / dette à traiter ultérieurement

- Le modèle RLS est volontairement minimal (mono-tenant, un seul utilisateur). Si un second utilisateur arrive, il faudra segmenter par `entreprise_id` et durcir les policies par rôle.
- Les contrôles d'unicité (ex: `clients.code` unique par entreprise, `marques.code` unique global) ne sont pas posés ici car l'app actuelle ne les enforce pas non plus — à valider avec votre père lors d'une phase de durcissement.
- Les colonnes `created_at` / `updated_at` sont des **ajouts** par rapport à l'iso-fonctionnel (faciles à backfill au cutover, valeur de support élevée).
