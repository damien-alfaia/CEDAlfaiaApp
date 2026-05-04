# Déploiement VPS — CEDAlfaiaApp v3

Templates et scripts pour déployer l'app `web/` sur le VPS.

## Setup initial du VPS (une fois)

```bash
# Sur Ubuntu/Debian récent
sudo apt update && sudo apt install -y curl git nginx certbot python3-certbot-nginx

# Node 22 LTS via NodeSource
curl -fsSL https://deb.nodesource.com/setup_22.x | sudo bash -
sudo apt install -y nodejs

# PM2 global
sudo npm install -g pm2

# Cloner le repo
sudo mkdir -p /var/www/cedalfaia /var/log/cedalfaia
sudo chown -R "$USER:$USER" /var/www/cedalfaia /var/log/cedalfaia
git clone https://github.com/damien-alfaia/CEDAlfaiaApp.git /var/www/cedalfaia
cd /var/www/cedalfaia
git checkout modernization

# Variables d'environnement de production
cd web
cp .env.example .env.local
# remplir avec les credentials Supabase prod
```

## Setup nginx + HTTPS

```bash
# Copier le vhost
sudo cp /var/www/cedalfaia/deploy/nginx.conf.example /etc/nginx/sites-available/cedalfaia.conf
# Adapter le server_name (remplacer garage.exemple.fr)
sudo nano /etc/nginx/sites-available/cedalfaia.conf
sudo ln -sf /etc/nginx/sites-available/cedalfaia.conf /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx

# Cert HTTPS Let's Encrypt
sudo certbot --nginx -d garage.exemple.fr
```

## Premier déploiement

```bash
cd /var/www/cedalfaia
bash deploy/deploy.sh
pm2 save
pm2 startup    # suivre la commande retournée pour activer le démarrage auto
```

## Déploiements suivants

```bash
cd /var/www/cedalfaia
bash deploy/deploy.sh
```

Pour automatiser via GitHub Actions plus tard : ajouter une étape `Deploy to VPS` qui SSH sur le VPS et lance `bash /var/www/cedalfaia/deploy/deploy.sh` (avec `BRANCH` en variable). Secrets nécessaires : `SSH_PRIVATE_KEY`, `SSH_HOST`, `SSH_USER`. À ajouter en Phase 8 (cutover) ou plus tôt si on veut du déploiement continu.

## Vérification post-déploiement

```bash
# Status PM2
pm2 status cedalfaia-web

# Logs en direct
pm2 logs cedalfaia-web

# Test HTTP
curl -I https://garage.exemple.fr
```

## Rollback rapide

```bash
cd /var/www/cedalfaia
git log --oneline -5    # repérer le commit précédent
git checkout <hash>
cd web && npm ci && npm run build
pm2 reload cedalfaia-web --update-env
```
