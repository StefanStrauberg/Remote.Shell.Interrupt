#!/usr/bin/env bash
# Same as deploy.sh, but uploads the release via `scp` (a tarball) instead of
# `rsync`, for environments where rsync fails or isn't available on the
# server/workstation. Everything else - rollback, symlink switch, restart,
# log dir, pruning - is identical; keep both scripts in sync if you change
# that shared logic.
#
# Usage:
#   deploy/deploy-scp.sh              # deploy the release publish.sh just built
#   deploy/deploy-scp.sh <tag>        # deploy a specific deploy/dist/<tag>
#   deploy/deploy-scp.sh --rollback   # repoint `current` back to the previous release
#
# Requires deploy/deploy.conf (copy deploy/deploy.conf.example and edit it).
#
# Assumes SERVER_USER has passwordless sudo on the server for the handful of
# root-only actions below (writing under /opt/rsi, systemctl, nginx reload).
# For tighter access than full sudo, restrict it to just these in
# /etc/sudoers.d/rsi-deploy instead:
#   SERVER_USER ALL=(root) NOPASSWD: /usr/bin/mkdir -p /opt/rsi/releases/*, \
#     /usr/bin/chown -R rsi\:rsi /opt/rsi/releases/*, /usr/bin/rm -rf /opt/rsi/releases/*, \
#     /usr/bin/ln -sfn /opt/rsi/releases/* /opt/rsi/current, \
#     /usr/bin/systemctl restart rsi-api, /usr/bin/systemctl status rsi-api, \
#     /usr/bin/systemctl reload nginx

set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."   # repo root

CONFIG="deploy/deploy.conf"
if [ ! -f "$CONFIG" ]; then
  echo "Missing $CONFIG - copy deploy/deploy.conf.example to deploy/deploy.conf and edit it." >&2
  exit 1
fi
# shellcheck disable=SC1090
source "$CONFIG"
: "${SERVER_HOST:?deploy.conf must set SERVER_HOST}"
: "${SERVER_USER:?deploy.conf must set SERVER_USER}"
REMOTE_BASE="${REMOTE_BASE:-/opt/rsi}"
SERVICE_NAME="${SERVICE_NAME:-rsi-api}"
KEEP_RELEASES="${KEEP_RELEASES:-5}"
REMOTE="$SERVER_USER@$SERVER_HOST"

if [ "${1:-}" = "--rollback" ]; then
  echo "==> Rolling back to the previous release on $SERVER_HOST"
  ssh "$REMOTE" bash -s -- "$REMOTE_BASE" "$SERVICE_NAME" <<'EOF'
    set -euo pipefail
    base="$1"; service="$2"
    cd "$base/releases"
    prev="$(ls -1t | sed -n 2p)"
    if [ -z "$prev" ]; then
      echo "No previous release to roll back to." >&2
      exit 1
    fi
    echo "Rolling back to $prev"
    sudo ln -sfn "$base/releases/$prev" "$base/current"
    sudo systemctl restart "$service"
    sudo systemctl status "$service" --no-pager -l | head -20
EOF
  exit 0
fi

TAG="${1:-}"
if [ -z "$TAG" ]; then
  TAG="$(cat deploy/dist/LATEST 2>/dev/null || true)"
fi
if [ -z "$TAG" ]; then
  echo "No release built yet - run deploy/publish.sh first, or pass a tag." >&2
  exit 1
fi
LOCAL_DIR="deploy/dist/$TAG"
if [ ! -d "$LOCAL_DIR" ]; then
  echo "Release $LOCAL_DIR not found." >&2
  exit 1
fi

echo "==> Packaging release $TAG"
TMP_DIR="$(mktemp -d)"
trap 'rm -rf "$TMP_DIR"' EXIT
LOCAL_ARCHIVE="$TMP_DIR/release.tar.gz"
tar -czf "$LOCAL_ARCHIVE" -C "$LOCAL_DIR" .

echo "==> Uploading release $TAG to $SERVER_HOST"
# Wipe and recreate the release dir first (matches rsync --delete's behavior
# in deploy.sh) so re-running the same tag can't leave stale files behind
# that were removed from the new build.
ssh "$REMOTE" "sudo rm -rf '$REMOTE_BASE/releases/$TAG' && sudo mkdir -p '$REMOTE_BASE/releases/$TAG' && sudo chown '$SERVER_USER' '$REMOTE_BASE/releases/$TAG'"
REMOTE_ARCHIVE="/tmp/rsi-release-$TAG.tar.gz"
scp "$LOCAL_ARCHIVE" "$REMOTE:$REMOTE_ARCHIVE"
ssh "$REMOTE" "tar -xzf '$REMOTE_ARCHIVE' -C '$REMOTE_BASE/releases/$TAG' && rm -f '$REMOTE_ARCHIVE'"

echo "==> Switching to $TAG and restarting $SERVICE_NAME"
ssh "$REMOTE" bash -s -- "$REMOTE_BASE" "$SERVICE_NAME" "$TAG" "$KEEP_RELEASES" <<'EOF'
  set -euo pipefail
  base="$1"; service="$2"; tag="$3"; keep="$4"

  sudo chown -R rsi:rsi "$base/releases/$tag"

  # Serilog writes a relative "logs/log-.txt" inside the working directory;
  # symlink it to the persistent shared/logs dir so log history survives
  # releases being swapped out.
  sudo rm -rf "$base/releases/$tag/api/logs"
  sudo ln -s "$base/shared/logs" "$base/releases/$tag/api/logs"

  sudo ln -sfn "$base/releases/$tag" "$base/current"
  sudo systemctl restart "$service"
  sudo systemctl reload nginx
  sleep 2
  sudo systemctl status "$service" --no-pager -l | head -20

  echo "==> Pruning old releases (keeping last $keep)"
  cd "$base/releases" && ls -1t | tail -n "+$((keep + 1))" | xargs -r sudo rm -rf
EOF

echo "==> Done. Check: curl -s http://$SERVER_HOST/health/live"
