#!/usr/bin/env bash
# Déploiement CEDAlfaiaApp v3 sur le VPS via Docker + Traefik.
# Lancer DEPUIS le VPS : bash /var/www/cedalfaia/deploy/deploy.sh
set -euo pipefail

REPO_DIR="/var/www/cedalfaia"
BRANCH="${BRANCH:-modernization}"

cd "$REPO_DIR"

echo "→ Pull dernière version de $BRANCH"
git fetch origin
git checkout "$BRANCH"
git pull --ff-only origin "$BRANCH"

cd "$REPO_DIR/deploy"

if [ ! -f .env ]; then
  echo "✗ deploy/.env manquant. Copier deploy/.env.example et remplir." >&2
  exit 1
fi

echo "→ Build + (re)start du conteneur"
docker compose up -d --build

echo "→ Status :"
docker compose ps

echo "→ Healthcheck (peut prendre quelques secondes au premier démarrage)..."
for i in 1 2 3 4 5 6; do
  status=$(docker inspect -f '{{.State.Health.Status}}' cedalfaia-web 2>/dev/null || echo "starting")
  echo "  [${i}/6] health=$status"
  if [ "$status" = "healthy" ]; then
    break
  fi
  sleep 5
done

echo "→ OK"
