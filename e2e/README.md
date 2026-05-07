# Tests E2E — Playwright

Tests bout-en-bout du flow utilisateur. Les tests publics tournent sans
configuration ; les tests authentifiés nécessitent un compte de test.

## Setup (une fois)

```bash
cd e2e
npm install
npx playwright install chromium      # télécharge le browser (~150 MB)
cp .env.example .env
# remplir E2E_BASE_URL (par défaut http://localhost:3000)
# pour les tests authentifiés : E2E_USER_EMAIL, E2E_USER_PASSWORD
```

## Créer un compte de test (staging)

Aller sur Supabase Dashboard → projet **staging** → Authentication →
Add user → Create new user. Cocher **Auto Confirm User** pour bypass la
confirmation email. Mettre les credentials dans `e2e/.env`.

## Exécution

```bash
# avec l'app en local (lancer `npm run dev` dans web/ en parallèle)
npm test

# ouvrir le UI mode interactif
npm run test:ui

# voir le dernier rapport
npm run report

# cibler la prod (uniquement tests publics, ne PAS exécuter les
# tests authentifiés en prod sauf si vous voulez réellement créer
# et supprimer des données)
E2E_BASE_URL=https://toniauto.dalfaia.fr npm test -- --grep "Pages publiques"
```

## Couverture actuelle

**Tests publics** (toujours actifs) :
- /login affiche le formulaire
- /signup affiche le formulaire
- Login avec mauvaises creds → erreur
- Signup avec mauvais code → erreur
- Page protégée → redirection vers /login

**Tests authentifiés** (skip si E2E_USER_* non défini) :
- Liste clients accessible
- Création + suppression client (soft delete)
- Navigation sidebar (Clients / Devis / Factures)

## À ajouter quand l'usage le justifiera

- Création complète d'un devis (lignes + services + main d'œuvre)
- Génération facture depuis devis + paiement + envoi comptable
- Upload document fournisseur
- Validation comparée de PDFs (snapshot testing)

## CI

À brancher sur `.github/workflows/ci.yml` (job e2e séparé) le jour où
l'on veut le bloquer comme garde-fou. Pour l'instant, exécution manuelle.
