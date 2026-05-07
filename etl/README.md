# ETL SQL Server → Supabase

Script de migration one-shot des données `TONIO_GARAGE` (SQL Server) vers
Supabase Postgres.

## Une fois pour toutes (par projet Supabase)

Avant la première migration, appliquer `db/etl_helpers.sql` au projet
Supabase cible (staging puis prod). Depuis le SQL editor du dashboard,
exécuter le contenu du fichier — il crée deux fonctions Postgres
`etl_truncate_all_data()` et `etl_reset_all_sequences()` accessibles
uniquement à la `service_role`.

## Préparation

```bash
cd etl
cp .env.example .env
# remplir MSSQL_* + SUPABASE_*
npm install
```

`MSSQL_HOST` doit pointer vers le SQL Server local (le conteneur
`cedalfaia-mssql` lancé sur le VPS, accessible en `127.0.0.1:11433` sur ce
même VPS).

## Vérifications préalables (dry run)

```bash
npm run migrate:dry
```

Affiche les compteurs source + cible sans rien modifier.

## Exécution réelle (idempotent — repassable)

```bash
npm run migrate
```

Étapes :

1. Connexions MSSQL + Supabase (service role)
2. `etl_truncate_all_data()` — vide les tables cibles
3. Insertions table par table dans l'ordre des FK :
   marques → modeles → parametrages → entreprises → fournisseurs →
   clients → voitures → pieces_vente → pieces_vente_lignes →
   pieces_vente_services → pieces_vente_paiements
4. `etl_reset_all_sequences()` — réaligne les `*_id_seq`
5. Comparaison source ↔ cible et affichage final

Les IDs sont **préservés** (insertion explicite) pour conserver les
NumDevis / NumFacture historiques.

## Skipped on purpose

- `CompteUtilisateurs` : les mots de passe ne sont pas migrables (hash
  custom). Les utilisateurs s'inscrivent via `/signup` avec le code
  d'inscription. La table `profiles` est peuplée automatiquement par le
  trigger `handle_new_auth_user`.
- `DocumentPieceVentes` : 0 ligne dans le `.bak`, donc rien à uploader
  vers Supabase Storage. Si jamais des documents apparaissent, à étendre.
- `Widgets` / `WidgetUtilisateurs` : la fonctionnalité widgets
  customisables est abandonnée dans la v3.
- `Salaries` / contrats / indispos / salaires : 0 ligne dans le `.bak`.
- `Fournisseurs` : 0 ligne. La table sera peuplée à la main si besoin.

## Mappings notables

- Casse : `PascalCase` MSSQL → `snake_case` Postgres.
- `Adresse_*` → `adresse_*` (ComplexType inline).
- Enum `TypePaiement` (int) → string : `0=especes 1=cheque 2=cb 3=virement`.
- `bit` → `boolean`. `real` → `double precision`. `datetime` → `timestamptz`.
- `varbinary` Logo/Entête : non migrés (NULL en source). Si un jour on
  les a, à uploader vers Supabase Storage avant insertion en DB.

## Rollback

`etl_truncate_all_data()` à la main depuis le SQL editor :

```sql
select etl_truncate_all_data();
```

Puis re-rejouer `npm run migrate`.
