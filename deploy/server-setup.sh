#!/usr/bin/env bash
# One-time bootstrap for a fresh Debian 12/13 server, meant to run during the
# single window where the server has internet access. Installs PostgreSQL
# and nginx from Debian's own repos, creates the app's system user, the
# release directory layout, the Postgres role/database, and installs (but
# does not start) the systemd unit and nginx site.
#
# After this script, no further step here needs internet: ongoing deploys
# (deploy/deploy.sh from a connected workstation) only ever `rsync` a
# self-contained .NET publish + static frontend build over SSH.
#
# Run as root (or via sudo) on the target server:
#   scp -r deploy/ admin@server:/tmp/rsi-deploy
#   ssh admin@server
#   cd /tmp/rsi-deploy && sudo ./server-setup.sh
#
# Idempotent: safe to re-run (e.g. after editing DB_PASSWORD below, or to
# pick up an updated nginx-rsi.conf / rsi-api.service).

set -euo pipefail

# --- Edit before running -----------------------------------------------
DB_NAME="remote_shell_interrupt"
DB_USER="rsi_app"
DB_PASSWORD="CHANGE_ME"          # must match ConnectionStrings__DefaultConnection in api.env
APP_USER="rsi"
APP_BASE="/opt/rsi"
# -------------------------------------------------------------------------

if [ "$(id -u)" -ne 0 ]; then
  echo "Run this as root (sudo ./server-setup.sh)." >&2
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "==> Updating packages and installing PostgreSQL + nginx"
apt-get update
apt-get upgrade -y
apt-get install -y postgresql nginx rsync ca-certificates

echo "==> Creating system user '$APP_USER'"
id -u "$APP_USER" &>/dev/null || useradd --system --no-create-home --shell /usr/sbin/nologin "$APP_USER"

echo "==> Creating release directory layout under $APP_BASE"
mkdir -p "$APP_BASE"/releases "$APP_BASE"/shared/logs
chown -R "$APP_USER:$APP_USER" "$APP_BASE"

echo "==> Seeding $APP_BASE/shared/api.env (edit this with real secrets!)"
if [ ! -f "$APP_BASE/shared/api.env" ]; then
  cp "$SCRIPT_DIR/api.env.example" "$APP_BASE/shared/api.env"
  sed -i "s/Database=remote_shell_interrupt;Username=rsi_app;Password=CHANGE_ME;/Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD;/" "$APP_BASE/shared/api.env"
  chown "$APP_USER:$APP_USER" "$APP_BASE/shared/api.env"
  chmod 600 "$APP_BASE/shared/api.env"
  echo "    Created. Still edit JwtSettings__Key and IdentitySeed__AdminPassword by hand:"
  echo "      \$EDITOR $APP_BASE/shared/api.env"
else
  echo "    Already exists, leaving it alone."
fi

echo "==> Creating Postgres role/database (if missing)"
sudo -u postgres psql -v ON_ERROR_STOP=1 -v db_user="$DB_USER" -v db_password="$DB_PASSWORD" -v db_name="$DB_NAME" <<'SQL'
DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = :'db_user') THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', :'db_user', :'db_password');
  END IF;
END
$$;
SELECT 'CREATE DATABASE ' || quote_ident(:'db_name') || ' OWNER ' || quote_ident(:'db_user')
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = :'db_name')\gexec
SQL

echo "==> Installing systemd unit"
install -m 644 "$SCRIPT_DIR/rsi-api.service" /etc/systemd/system/rsi-api.service
systemctl daemon-reload
systemctl enable rsi-api
echo "    Enabled (will start on boot). Not started yet - no release deployed."

echo "==> Installing nginx site"
install -m 644 "$SCRIPT_DIR/nginx-rsi.conf" /etc/nginx/sites-available/rsi
ln -sf /etc/nginx/sites-available/rsi /etc/nginx/sites-enabled/rsi
rm -f /etc/nginx/sites-enabled/default
nginx -t
systemctl reload nginx

cat <<'EOF'

==> Server bootstrap complete.

Still needed before the first deploy:
  1. Edit /opt/rsi/shared/api.env - set JwtSettings__Key (32+ random chars,
     e.g. `openssl rand -base64 48`) and IdentitySeed__AdminPassword.
  2. Optionally restrict SSH deploy access - see deploy/deploy.sh's header
     comment for a minimal sudoers snippet.
  3. From your workstation: deploy/publish.sh && deploy/deploy.sh

/opt/rsi/current does not exist yet - rsi-api.service will fail to start
until the first `deploy/deploy.sh` run creates it. That's expected.
EOF
