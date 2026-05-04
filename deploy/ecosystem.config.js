// PM2 ecosystem file
// Lancer sur le VPS depuis /var/www/cedalfaia/web (chemin à adapter) :
//   pm2 start ../deploy/ecosystem.config.js --env production
//   pm2 save
//   pm2 startup        # à lancer une fois pour activer le démarrage auto
module.exports = {
  apps: [
    {
      name: "cedalfaia-web",
      cwd: "/var/www/cedalfaia/web",
      script: ".next/standalone/server.js",
      instances: 1,
      exec_mode: "fork",
      env_production: {
        NODE_ENV: "production",
        PORT: 3000,
        HOSTNAME: "127.0.0.1",
      },
      max_memory_restart: "512M",
      error_file: "/var/log/cedalfaia/web-error.log",
      out_file: "/var/log/cedalfaia/web-out.log",
      time: true,
    },
  ],
};
