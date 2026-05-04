# Mapping SQL Server → PostgreSQL (Supabase)

Document de référence pour la migration du schéma `TONIO_GARAGE` (SQL Server) vers Supabase Postgres.

## Conventions générales

| Aspect | SQL Server / EF6 actuel | Supabase / Postgres v3 |
|---|---|---|
| Casse des identifiants | `PascalCase` (tables `Clients`, colonnes `IsProspect`) | `snake_case` (tables `clients`, colonnes `is_prospect`) |
| Pluriel des tables | Mixte (`Clients` mais `PiecesVente`) | Pluriel systématique (`clients`, `pieces_vente`) |
| Clé primaire | `[Key] int ID` (IDENTITY) | `id bigserial primary key` |
| Clés étrangères | `int? FooID` + `[ForeignKey]` | `foo_id bigint references foos(id)` |

**Choix `bigserial` vs `uuid`** : on garde `bigserial` (entier auto-incrément) pour préserver les numéros lisibles existants (`NumDevis`, `NumFacture`) et faciliter l'ETL (les FK numériques restent stables). Si un besoin d'URL publique apparaît plus tard, on ajoutera une colonne secondaire `public_id uuid` ciblée.

## Mapping des types

| Type C# / EF (SQL Server) | Type Postgres |
|---|---|
| `int` | `integer` |
| `int?` | `integer` (nullable) |
| `bool` | `boolean` |
| `bool?` | `boolean` (nullable) |
| `float` | `double precision` |
| `float?` | `double precision` (nullable) |
| `DateTime` | `timestamptz` |
| `DateTime?` | `timestamptz` (nullable) |
| `string` (sans `[StringLength]`) | `text` |
| `[StringLength(n)] string` + `[Column("VARCHAR")]` | `varchar(n)` |
| `[Required] string` | `not null` |
| `byte[]` | **Supabase Storage** + colonne `storage_path text` (voir plus bas) |
| Enum C# (ex: `TypePaiement`) | Postgres `enum` dédié |

## Type complexe `Adresse` (ComplexType EF)

EF6 inline les colonnes du ComplexType dans la table parent avec préfixe : `Adresse_Ligne1`, `Adresse_Ligne2`, etc.

En Postgres, on conserve la même approche (colonnes inline) avec préfixe `adresse_` :

```
adresse_ligne1 varchar(50)
adresse_ligne2 varchar(50)
adresse_ligne3 varchar(50)
adresse_code_postal varchar(50)
adresse_ville varchar(50)
adresse_pays varchar(50)
```

Tables concernées : `clients`, `fournisseurs`, `entreprises`, `parametrages`, `salaries`, `profiles` (ex-`CompteUtilisateurs`).

Alternative considérée puis écartée : table `addresses` séparée avec FK. Décision : non, ça complique l'ETL et n'apporte rien fonctionnellement (les adresses ne sont jamais réutilisées entre entités).

## Stockage des `byte[]` → Supabase Storage

Trois colonnes binaires existent en SQL Server, toutes migrées vers Supabase Storage :

| Source | Bucket Storage | Chemin | Colonne DB de référence |
|---|---|---|---|
| `DocumentPieceVente.Doc` | `documents` (privé) | `pieces-vente/{piece_vente_id}/{document_id}` | `documents.storage_path text` |
| `Parametrage.Logo` | `parametrage` (privé) | `logo/{parametrage_id}` | `parametrages.logo_path text` |
| `Parametrage.Entete` | `parametrage` (privé) | `entete/{parametrage_id}` | `parametrages.entete_path text` |

`DocFormatFichierBase64` (qui semble stocker le format/MIME du fichier) → renommée `mime_type text` ou conservée telle quelle ? Décision : on conserve `format_fichier text` (la valeur stockée est l'extension/MIME, l'app la consomme telle quelle pour servir le fichier).

Lors de l'ETL (Phase 7), le script lit le `byte[]` SQL Server, l'upload dans le bucket Supabase, et stocke uniquement le chemin en DB.

## Authentification — abandon de `CompteUtilisateurs`

La table `CompteUtilisateurs` est **supprimée** ; remplacée par :

1. **`auth.users`** (table managée par Supabase Auth) — gère email, password (hashé bcrypt par Supabase), JWT, refresh tokens.
2. **`profiles`** (table custom v3) — un profil par `auth.users`, contient les informations métier (rôle, prénom, nom, adresse, entreprise) :

```
profiles (
  id uuid primary key references auth.users(id) on delete cascade,
  email text not null,
  prenom varchar(256),
  nom text,
  role user_role not null default 'administrateur',
  entreprise_id bigint references entreprises(id),
  adresse_ligne1 varchar(50),
  adresse_ligne2 varchar(50),
  adresse_ligne3 varchar(50),
  adresse_code_postal varchar(50),
  adresse_ville varchar(50),
  adresse_pays varchar(50),
  created_at timestamptz default now(),
  updated_at timestamptz default now()
)
```

**Conséquence** : `WidgetUtilisateur.CompteUtilisateurID int?` devient `widgets_utilisateurs.profile_id uuid` (référence vers `profiles.id`).

**Migration des mots de passe** : impossible (le hash actuel est custom, non rejouable côté Supabase). Au cutover, votre père reçoit un mot de passe initial via email magic link, qu'il change à la première connexion.

## Mapping enum par enum

### TypePaiement
```sql
create type type_paiement as enum ('especes', 'cheque', 'cb', 'virement');
```
Mapping ETL : `0→especes`, `1→cheque`, `2→cb`, `3→virement`.

### TypeDocument
```sql
create type type_document as enum ('devis', 'facture', 'bon_livraison_fournisseur', 'devis_fournisseur');
```
Mapping ETL : `0→devis`, `1→facture`, `2→bon_livraison_fournisseur`, `3→devis_fournisseur`.

### TypeIndisponibilite
```sql
create type type_indisponibilite as enum ('absence', 'conges', 'ecole', 'autre');
```
Mapping ETL : `0→absence`, `1→conges`, `2→ecole`, `3→autre`.

### CompteUtilisateurRole → user_role
```sql
create type user_role as enum ('super_admin', 'administrateur', 'secretaire', 'technicien');
```
Mapping ETL : `0→super_admin`, `1→administrateur`, `2→secretaire`, `3→technicien`.

## Soft-delete

L'app actuelle utilise `DateSuppression timestamptz null` pour les suppressions logiques (`Clients`, `Voitures`, `Fournisseurs`). On conserve la même mécanique (la couche métier filtre `where date_suppression is null`).

## Audit

L'app actuelle n'a pas de colonnes d'audit systématiques. On en ajoute deux pour faciliter le support :

```
created_at timestamptz default now() not null
updated_at timestamptz default now() not null
```

Ce sont les seules **colonnes nouvelles** par rapport à l'iso-fonctionnel. Toléré car aucun impact métier, et la valeur de troubleshooting est forte. Trigger Postgres pour mettre `updated_at` à jour automatiquement.

## Cascade delete

EF supprime `OneToManyCascadeDeleteConvention` (`OnModelCreating`). En Postgres, on calque ce comportement : pas de `on delete cascade` par défaut. Exceptions explicites :

- `profiles.id → auth.users.id` : `on delete cascade` (si l'auth.user disparaît, le profil suit).
- `pieces_vente_lignes.piece_vente_id → pieces_vente.id` : `on delete cascade` (les lignes appartiennent au document parent, pas de sens à les conserver orphelines).
- `pieces_vente_services.piece_vente_id` : idem cascade.
- `pieces_vente_paiements.piece_vente_id` : idem cascade.
- `documents.piece_vente_id` : idem cascade.
- `salarie_contrats.salarie_id` : `on delete cascade`.
- `salarie_indisponibilites.salarie_contrat_id` : idem.
- `salarie_salaires.salarie_contrat_id` : idem.
- `widgets_utilisateurs.profile_id` : `on delete cascade`.
- `widgets_utilisateurs.widget_id` : `on delete cascade`.

Toutes les autres FK : pas de cascade (suppression manuelle ou bloquée par contrainte).

## Indexes

Indexes systématiques :
- Toutes les FK
- `clients.email`, `clients.telephone`, `clients.code` (recherche)
- `voitures.immatriculation` (recherche)
- `pieces_vente.num_devis`, `pieces_vente.num_facture` (recherche, tri)
- `pieces_vente.date_devis`, `pieces_vente.date_facture` (filtres période)
- `pieces_vente.client_id`, `pieces_vente.voiture_id` (jointures)
- `profiles.email` (recherche / login)

## RLS — Modèle d'autorisation

Vu qu'il y a **un seul utilisateur** (votre père) et qu'on ne fait pas de multi-tenant, le modèle RLS reste minimal :

> **Toute opération CRUD sur les tables métier est autorisée si `auth.uid()` est dans `profiles`.**

Cela suffit. Les rôles (`super_admin`, etc.) servent côté Next.js pour masquer/griser des actions UI (ex: seul `administrateur` voit le module `parametrages`). Pas de RLS différenciée par rôle pour l'instant — on l'ajoutera si un second utilisateur arrive.

Voir `db/rls.sql` pour les policies exactes.
