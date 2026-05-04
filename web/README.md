# CEDAlfaiaApp v3 — App web

Application Next.js 16 + Supabase, livrable progressif du plan de modernisation.

## Stack

- **Next.js 16** (App Router, RSC, Turbopack)
- **React 19** + **TypeScript 6**
- **Tailwind CSS 4** + **shadcn/ui** (composants ajoutés à la demande via `npx shadcn@latest add <name>`)
- **Supabase** : Auth (cookies SSR), Postgres (via @supabase/ssr), Storage
- **ESLint** (next/core-web-vitals + next/typescript) + **Prettier**

## Démarrer en local

Prérequis : Node 22+, npm 11+.

```bash
cd web
cp .env.example .env.local
# remplir NEXT_PUBLIC_SUPABASE_URL et NEXT_PUBLIC_SUPABASE_ANON_KEY
# (Supabase Dashboard → Project Settings → API)

npm install
npm run dev
# → http://localhost:3000
```

Sans `.env.local` configuré, le middleware d'auth se désactive (page d'accueil accessible) — pratique pour tester le scaffold avant d'avoir un projet Supabase.

## Scripts

| Commande               | Rôle                                             |
| ---------------------- | ------------------------------------------------ |
| `npm run dev`          | Dev server (Turbopack)                           |
| `npm run build`        | Build production (mode `standalone` pour le VPS) |
| `npm run start`        | Lance le build de prod                           |
| `npm run lint`         | ESLint                                           |
| `npm run typecheck`    | `tsc --noEmit`                                   |
| `npm run format`       | Prettier write                                   |
| `npm run format:check` | Prettier check                                   |

## Architecture du dossier

```
web/
├── app/                    # App Router (layouts, pages, route handlers)
│   ├── layout.tsx
│   ├── page.tsx            # Accueil (statut env)
│   ├── login/page.tsx      # Placeholder (Phase 3)
│   └── globals.css
├── components/
│   └── ui/                 # Composants shadcn (à ajouter via CLI)
├── lib/
│   ├── utils.ts            # cn() helper
│   └── supabase/
│       ├── client.ts       # createBrowserClient (Client Components)
│       ├── server.ts       # createServerClient (RSC, Route Handlers)
│       └── middleware.ts   # updateSession (refresh tokens)
├── middleware.ts           # Protection des routes
├── components.json         # Config shadcn
└── next.config.ts          # output: "standalone"
```

## Étapes utilisateur restantes (Phase 2)

1. **Créer 2 projets Supabase** (free tier) :
   - `cedalfaia-staging` — pour les tests
   - `cedalfaia-prod` — production
2. **Appliquer le schéma DB** (depuis `db/` à la racine du repo) : `schema.sql` → `storage.sql` → `rls.sql`
3. **Renseigner `.env.local`** avec les credentials du projet **staging**
4. **Préparer le VPS** : Node 22 LTS, PM2, nginx, certbot (voir `deploy/README.md`)
5. **DNS** : pointer un sous-domaine vers le VPS (ex : `garage.votre-domaine.tld`)
6. **GitHub Secrets** pour la CI/CD (voir `.github/workflows/ci.yml`)

## Ajouter un composant shadcn/ui

```bash
npx shadcn@latest add button
npx shadcn@latest add card input label form
```

## Convention de code

- Server Components par défaut (`"use client"` uniquement si nécessaire)
- Lecture DB côté serveur via `lib/supabase/server.ts`
- Mutations via Server Actions ou Route Handlers — pas d'appel direct depuis le client (sauf realtime)
- Validation runtime : Zod (à ajouter dès la Phase 4)
