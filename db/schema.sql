-- =============================================================================
-- CEDAlfaiaApp v3 — Schéma PostgreSQL pour Supabase
-- =============================================================================
-- À exécuter sur un projet Supabase vide.
-- Voir db/MAPPING.md pour les choix de design.
-- Ordre d'exécution : schema.sql → storage.sql → rls.sql
-- =============================================================================

set search_path = public;

-- -----------------------------------------------------------------------------
-- 1. ENUMS
-- -----------------------------------------------------------------------------

create type user_role as enum (
  'super_admin',
  'administrateur',
  'secretaire',
  'technicien'
);

create type type_paiement as enum (
  'especes',
  'cheque',
  'cb',
  'virement'
);

create type type_document as enum (
  'devis',
  'facture',
  'bon_livraison_fournisseur',
  'devis_fournisseur'
);

create type type_indisponibilite as enum (
  'absence',
  'conges',
  'ecole',
  'autre'
);

-- -----------------------------------------------------------------------------
-- 2. TRIGGER updated_at générique
-- -----------------------------------------------------------------------------

create or replace function set_updated_at()
returns trigger
language plpgsql
as $$
begin
  new.updated_at = now();
  return new;
end;
$$;

-- -----------------------------------------------------------------------------
-- 3. TABLES — sans dépendances FK
-- -----------------------------------------------------------------------------

create table parametrages (
  id                            bigserial primary key,
  nom_entreprise                text,
  email_entreprise              text,
  telephone_entreprise          text,
  portable_entreprise           text,
  adresse_ligne1                varchar(50),
  adresse_ligne2                varchar(50),
  adresse_ligne3                varchar(50),
  adresse_code_postal           varchar(50),
  adresse_ville                 varchar(50),
  adresse_pays                  varchar(50),
  logo_path                     text,
  entete_path                   text,
  tva                           double precision not null default 0.2,
  main_doeuvre_montant_horaire  double precision not null default 0,
  email_comptable               text,
  is_save_piece_de_vente        boolean not null default false,
  siret                         text,
  code_ape                      text,
  tva_intra_communautaire       text,
  libelle_bas_de_page           text,
  email_envoi_smtp              text,
  protocole_smtp                text,
  port_smtp                     integer,
  serveur_smtp                  text,
  email_smtp                    text,
  password_smtp                 text,
  is_ssl                        boolean not null default false,
  objectif_annuel               double precision,
  created_at                    timestamptz not null default now(),
  updated_at                    timestamptz not null default now()
);
create trigger trg_parametrages_updated_at before update on parametrages
  for each row execute function set_updated_at();

create table marques (
  id        bigserial primary key,
  code      varchar(5)   not null,
  libelle   varchar(500) not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);
create trigger trg_marques_updated_at before update on marques
  for each row execute function set_updated_at();

create table fournisseurs (
  id                  bigserial primary key,
  nom                 varchar(256) not null,
  adresse_ligne1      varchar(50),
  adresse_ligne2      varchar(50),
  adresse_ligne3      varchar(50),
  adresse_code_postal varchar(50),
  adresse_ville       varchar(50),
  adresse_pays        varchar(50),
  commentaire         varchar(5000) not null default '',
  date_suppression    timestamptz,
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_fournisseurs_updated_at before update on fournisseurs
  for each row execute function set_updated_at();

create table rendez_vous (
  id                bigserial primary key,
  sujet             text,
  date_heure_debut  timestamptz,
  date_heure_fin    timestamptz,
  duree             integer,
  commentaire       text,
  created_at        timestamptz not null default now(),
  updated_at        timestamptz not null default now()
);
create trigger trg_rendez_vous_updated_at before update on rendez_vous
  for each row execute function set_updated_at();

create table widgets (
  id                  bigserial primary key,
  libelle             text,
  html_name           text,
  html_panel_uid      text,
  html_partial_widget text,
  html_width          text,
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_widgets_updated_at before update on widgets
  for each row execute function set_updated_at();

create table salaries (
  id                  bigserial primary key,
  nom                 text,
  prenom              text,
  date_naissance      date not null,
  adresse_ligne1      varchar(50),
  adresse_ligne2      varchar(50),
  adresse_ligne3      varchar(50),
  adresse_code_postal varchar(50),
  adresse_ville       varchar(50),
  adresse_pays        varchar(50),
  telephone           text,
  portable            text,
  email               text,
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_salaries_updated_at before update on salaries
  for each row execute function set_updated_at();

-- -----------------------------------------------------------------------------
-- 4. TABLES — avec FK simples
-- -----------------------------------------------------------------------------

create table entreprises (
  id              bigserial primary key,
  nom             varchar(256) not null,
  adresse_ligne1  varchar(50),
  adresse_ligne2  varchar(50),
  adresse_ligne3  varchar(50),
  adresse_code_postal varchar(50),
  adresse_ville   varchar(50),
  adresse_pays    varchar(50),
  siren           text,
  siret           text,
  commentaire     varchar(5000),
  date_creation   timestamptz not null default now(),
  is_actif        boolean not null default true,
  parametrage_id  bigint references parametrages(id),
  created_at      timestamptz not null default now(),
  updated_at      timestamptz not null default now()
);
create trigger trg_entreprises_updated_at before update on entreprises
  for each row execute function set_updated_at();
create index idx_entreprises_parametrage_id on entreprises(parametrage_id);

create table profiles (
  id                  uuid primary key references auth.users(id) on delete cascade,
  email               text not null,
  prenom              varchar(256),
  nom                 text,
  role                user_role not null default 'administrateur',
  entreprise_id       bigint references entreprises(id),
  adresse_ligne1      varchar(50),
  adresse_ligne2      varchar(50),
  adresse_ligne3      varchar(50),
  adresse_code_postal varchar(50),
  adresse_ville       varchar(50),
  adresse_pays        varchar(50),
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_profiles_updated_at before update on profiles
  for each row execute function set_updated_at();
create index idx_profiles_entreprise_id on profiles(entreprise_id);
create unique index idx_profiles_email on profiles(email);

create table modeles (
  id          bigserial primary key,
  marque_id   bigint references marques(id),
  libelle     varchar(500) not null,
  created_at  timestamptz not null default now(),
  updated_at  timestamptz not null default now()
);
create trigger trg_modeles_updated_at before update on modeles
  for each row execute function set_updated_at();
create index idx_modeles_marque_id on modeles(marque_id);

create table clients (
  id                              bigserial primary key,
  code                            varchar(256),
  nom                             varchar(256) not null,
  prenom                          varchar(256),
  adresse_ligne1                  varchar(50),
  adresse_ligne2                  varchar(50),
  adresse_ligne3                  varchar(50),
  adresse_code_postal             varchar(50),
  adresse_ville                   varchar(50),
  adresse_pays                    varchar(50),
  informations_complementaires    varchar(5000),
  telephone                       varchar(100),
  email                           varchar(256),
  remise                          double precision,
  date_suppression                timestamptz,
  is_prospect                     boolean not null default true,
  entreprise_id                   bigint references entreprises(id),
  created_at                      timestamptz not null default now(),
  updated_at                      timestamptz not null default now()
);
create trigger trg_clients_updated_at before update on clients
  for each row execute function set_updated_at();
create index idx_clients_entreprise_id on clients(entreprise_id);
create index idx_clients_email on clients(email);
create index idx_clients_telephone on clients(telephone);
create index idx_clients_code on clients(code);
create index idx_clients_nom_prenom on clients(nom, prenom);

create table voitures (
  id              bigserial primary key,
  immatriculation varchar(20),
  modele_id       bigint references modeles(id),
  client_id       bigint references clients(id),
  is_principale   boolean not null default false,
  date_suppression timestamptz,
  created_at      timestamptz not null default now(),
  updated_at      timestamptz not null default now()
);
create trigger trg_voitures_updated_at before update on voitures
  for each row execute function set_updated_at();
create index idx_voitures_modele_id on voitures(modele_id);
create index idx_voitures_client_id on voitures(client_id);
create index idx_voitures_immatriculation on voitures(immatriculation);

-- -----------------------------------------------------------------------------
-- 5. TABLES — Pièces de vente (devis / factures)
-- -----------------------------------------------------------------------------

create table pieces_vente (
  id                          bigserial primary key,
  client_id                   bigint references clients(id),
  voiture_id                  bigint references voitures(id),
  rendez_vous_id              bigint references rendez_vous(id),
  main_doeuvre_duree          integer       not null default 0,
  main_doeuvre_montant_horaire double precision not null default 0,
  date_devis                  timestamptz,
  num_devis                   integer,
  date_facture                timestamptz,
  num_facture                 integer,
  kilometrage                 integer       not null default 0,
  is_facture_annule           boolean       not null default false,
  date_heure_annulation       timestamptz,
  commentaire_annulation      text,
  is_valide                   boolean       not null default false,
  date_heure_validation       timestamptz,
  is_envoye_comptable         boolean       not null default false,
  is_devis_envoye             boolean       not null default false,
  remise                      double precision,
  total_ttc                   double precision not null default 0,
  montant_tva                 double precision not null default 0,
  total_ht                    double precision not null default 0,
  benefice_ttc                double precision not null default 0,
  reste_a_payer               double precision not null default 0,
  created_at                  timestamptz   not null default now(),
  updated_at                  timestamptz   not null default now()
);
create trigger trg_pieces_vente_updated_at before update on pieces_vente
  for each row execute function set_updated_at();
create index idx_pieces_vente_client_id      on pieces_vente(client_id);
create index idx_pieces_vente_voiture_id     on pieces_vente(voiture_id);
create index idx_pieces_vente_rendez_vous_id on pieces_vente(rendez_vous_id);
create index idx_pieces_vente_num_devis      on pieces_vente(num_devis);
create index idx_pieces_vente_num_facture    on pieces_vente(num_facture);
create index idx_pieces_vente_date_devis     on pieces_vente(date_devis);
create index idx_pieces_vente_date_facture   on pieces_vente(date_facture);

create table pieces_vente_lignes (
  id              bigserial primary key,
  piece_vente_id  bigint not null references pieces_vente(id) on delete cascade,
  fournisseur_id  bigint references fournisseurs(id),
  libelle         varchar(500),
  remise          double precision not null default 0,
  prix_garage_ht  double precision not null default 0,
  prix_garage_ttc double precision not null default 0,
  prix_client_ht  double precision not null default 0,
  prix_client_ttc double precision not null default 0,
  quantite        integer not null default 1,
  created_at      timestamptz not null default now(),
  updated_at      timestamptz not null default now()
);
create trigger trg_pieces_vente_lignes_updated_at before update on pieces_vente_lignes
  for each row execute function set_updated_at();
create index idx_pieces_vente_lignes_piece_vente_id on pieces_vente_lignes(piece_vente_id);
create index idx_pieces_vente_lignes_fournisseur_id on pieces_vente_lignes(fournisseur_id);

create table pieces_vente_services (
  id              bigserial primary key,
  piece_vente_id  bigint not null references pieces_vente(id) on delete cascade,
  libelle         varchar(500),
  prix_client_ht  double precision not null default 0,
  prix_client_ttc double precision not null default 0,
  quantite        integer not null default 1,
  created_at      timestamptz not null default now(),
  updated_at      timestamptz not null default now()
);
create trigger trg_pieces_vente_services_updated_at before update on pieces_vente_services
  for each row execute function set_updated_at();
create index idx_pieces_vente_services_piece_vente_id on pieces_vente_services(piece_vente_id);

create table pieces_vente_paiements (
  id              bigserial primary key,
  piece_vente_id  bigint not null references pieces_vente(id) on delete cascade,
  type_paiement   type_paiement not null,
  date            timestamptz not null,
  montant         double precision not null,
  created_at      timestamptz not null default now(),
  updated_at      timestamptz not null default now()
);
create trigger trg_pieces_vente_paiements_updated_at before update on pieces_vente_paiements
  for each row execute function set_updated_at();
create index idx_pieces_vente_paiements_piece_vente_id on pieces_vente_paiements(piece_vente_id);

create table documents (
  id                  bigserial primary key,
  piece_vente_id      bigint not null references pieces_vente(id) on delete cascade,
  libelle             text,
  type_document       type_document not null,
  storage_path        text,
  format_fichier      text,
  date_creation       timestamptz default now(),
  date_modification   timestamptz default now(),
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_documents_updated_at before update on documents
  for each row execute function set_updated_at();
create index idx_documents_piece_vente_id on documents(piece_vente_id);
create index idx_documents_type_document on documents(type_document);

-- -----------------------------------------------------------------------------
-- 6. TABLES — Salariés
-- -----------------------------------------------------------------------------

create table salarie_contrats (
  id            bigserial primary key,
  date_debut    date not null,
  date_fin      date,
  type_contrat  text,
  salarie_id    bigint references salaries(id) on delete cascade,
  created_at    timestamptz not null default now(),
  updated_at    timestamptz not null default now()
);
create trigger trg_salarie_contrats_updated_at before update on salarie_contrats
  for each row execute function set_updated_at();
create index idx_salarie_contrats_salarie_id on salarie_contrats(salarie_id);

create table salarie_indisponibilites (
  id                    bigserial primary key,
  date_debut            timestamptz not null,
  date_fin              timestamptz not null,
  type_indisponibilite  type_indisponibilite not null,
  motif                 text,
  salarie_contrat_id    bigint references salarie_contrats(id) on delete cascade,
  created_at            timestamptz not null default now(),
  updated_at            timestamptz not null default now()
);
create trigger trg_salarie_indisponibilites_updated_at before update on salarie_indisponibilites
  for each row execute function set_updated_at();
create index idx_salarie_indisponibilites_contrat_id on salarie_indisponibilites(salarie_contrat_id);

create table salarie_salaires (
  id                  bigserial primary key,
  date_debut          date not null,
  date_fin            date not null,
  date_paiement       date not null,
  salaire_net         double precision not null,
  salarie_contrat_id  bigint references salarie_contrats(id) on delete cascade,
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_salarie_salaires_updated_at before update on salarie_salaires
  for each row execute function set_updated_at();
create index idx_salarie_salaires_contrat_id on salarie_salaires(salarie_contrat_id);

-- -----------------------------------------------------------------------------
-- 7. TABLES — Widgets utilisateurs
-- -----------------------------------------------------------------------------

create table widgets_utilisateurs (
  id                  bigserial primary key,
  widget_id           bigint references widgets(id) on delete cascade,
  profile_id          uuid references profiles(id) on delete cascade,
  titre               text,
  html_owner_zone_uid text,
  html_index          integer,
  html_top            text,
  html_left           text,
  date_debut          timestamptz,
  date_fin            timestamptz,
  is_mois_en_cours    boolean,
  is_annee_en_cours   boolean,
  created_at          timestamptz not null default now(),
  updated_at          timestamptz not null default now()
);
create trigger trg_widgets_utilisateurs_updated_at before update on widgets_utilisateurs
  for each row execute function set_updated_at();
create index idx_widgets_utilisateurs_widget_id on widgets_utilisateurs(widget_id);
create index idx_widgets_utilisateurs_profile_id on widgets_utilisateurs(profile_id);

-- -----------------------------------------------------------------------------
-- Fin du schéma. Suite : storage.sql puis rls.sql.
-- -----------------------------------------------------------------------------
