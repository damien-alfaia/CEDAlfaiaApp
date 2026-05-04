#!/usr/bin/env bash
# Déploiement CEDAlfaiaApp v3 sur le VPS.
# Lancer DEPUIS le VPS, dans /var/www/cedalfaia (chemin à adapter).
# Suppose un git clone initial : git clone <repo> /var/www/cedalfaia
set -euo pipefail

REPO_DIR="/var/www/cedalfaia"
BRANCH="${BRANCH:-modernization}"

cd "$REPO_DIR"

echo "→ Pull dernière version de $BRANCH"
git fetch origin
git checkout "$BRANCH"
git pull --ff-only origin "$BRANCH"

cd "$REPO_DIR/web"

echo "→ Install dépendances (production)"
npm ci

echo "→ Build Next.js (standalone)"
npm run build

# Next standalone: copier les fichiers statiques et public au bon endroit
echo "→ Copie public/ et .next/static dans .next/standalone"
mkdir -p .next/standalone/.next
cp -r public .next/standalone/public 2>/dev/null || true
cp -r .next/static .next/standalone/.next/static

echo "→ Reload PM2"
pm2 reload cedalfaia-web --update-env || pm2 start ../deploy/ecosystem.config.js --env production

echo "→ OK. Status :"
pm2 status cedalfaia-web
