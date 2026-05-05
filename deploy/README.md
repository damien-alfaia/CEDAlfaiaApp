# Déploiement VPS — CEDAlfaiaApp v3

L'app est déployée en **conteneur Docker** sur le VPS, **derrière le Traefik existant** (Hostinger), avec Let's Encrypt automatique via le challenge ACME HTTP de Traefik.

## Setup initial du VPS (une fois)

Le VPS Hostinger arrive avec Docker + Traefik déjà installés. Il reste juste à cloner le repo et configurer l'env :

```bash
# SSH root@<vps>
mkdir -p /var/www
git clone https://github.com/damien-alfaia/CEDAlfaiaApp.git /var/www/cedalfaia
cd /var/www/cedalfaia
git checkout modernization

# Variables d'environnement de prod
cp deploy/.env.example deploy/.env
# Renseigner DOMAIN, NEXT_PUBLIC_SUPABASE_URL, NEXT_PUBLIC_SUPABASE_ANON_KEY
nano deploy/.env
chmod 600 deploy/.env
```

## DNS

L'enregistrement A du domaine doit pointer vers l'IP publique du VPS :

```
toniauto.dalfaia.fr.   A   187.124.42.207
```

À configurer chez le registrar / DNS provider du domaine.

## Premier déploiement

```bash
cd /var/www/cedalfaia
bash deploy/deploy.sh
```

Au premier démarrage, Traefik négocie un certificat Let's Encrypt (HTTP-01) — ça peut prendre 30-60 secondes.

## Déploiements suivants

Pareil :

```bash
cd /var/www/cedalfaia
bash deploy/deploy.sh
```

`docker compose up -d --build` rebuild l'image et redéploie sans coupure (rolling update simple).

## Vérification

```bash
# Conteneur sain ?
docker ps --filter name=cedalfaia-web --format "{{.Names}}\t{{.Status}}"

# Logs en direct
docker logs -f cedalfaia-web

# Test HTTP
curl -I https://toniauto.dalfaia.fr
```

## Stack en place

```
Internet
   │
   ├─ :80  ──┐
   │         ├─→ Traefik (network_mode: host, conteneur Docker préconfiguré Hostinger)
   ├─ :443 ──┘                │
                              │ rule: Host(`toniauto.dalfaia.fr`)
                              │ tls: letsencrypt
                              ↓
                         127.0.0.1:3000
                              │
                              ↓
                  cedalfaia-web (conteneur Docker)
                  • Next.js 16 standalone
                  • Healthcheck /login
                  • restart: unless-stopped
                              │
                              ↓
                          Supabase (Postgres + Auth + Storage)
```

## Rollback rapide

```bash
cd /var/www/cedalfaia
git log --oneline -5    # repérer le commit précédent
git checkout <hash>
bash deploy/deploy.sh
```

Ou directement sur l'image Docker précédente si elle est en cache local :

```bash
docker tag cedalfaia-web:latest cedalfaia-web:rolling-back  # backup
# checkout commit précédent puis rebuild
```

## Auto-deploy depuis GitHub Actions (à venir, Phase 8)

Pour automatiser : ajouter une étape `Deploy` dans `.github/workflows/ci.yml` qui SSH sur le VPS et lance `bash /var/www/cedalfaia/deploy/deploy.sh`. Secrets nécessaires :
- `SSH_PRIVATE_KEY` (clé dédiée au déploiement)
- `SSH_HOST=187.124.42.207`
- `SSH_USER=root` (ou un user dédié au déploiement)

À mettre en place quand on aura suffisamment de cycles de déploiement pour que l'investissement soit rentable.
