#!/usr/bin/env bash
# Builds one release locally: a self-contained linux-x64 API publish (the
# target server needs no .NET runtime installed at all) plus the frontend
# production build. Run from a machine with internet + the .NET 9 SDK +
# Node.js (matching client/package.json's engines) - typically your
# workstation, not the isolated server.
#
# Output: deploy/dist/<timestamp>_<git-sha>/{api,client}/
# Next step: deploy/deploy.sh (defaults to the release this script just built).

set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."   # repo root

TAG="$(date +%Y%m%d_%H%M%S)_$(git rev-parse --short HEAD)"
OUT="deploy/dist/$TAG"
API_CSPROJ="src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API/Remote.Shell.Interrupt.Storehouse.API.csproj"

echo "==> Publishing API (self-contained, linux-x64) -> $OUT/api"
# InvariantGlobalization=true: skips the ICU dependency entirely, so the
# target Debian box doesn't need a matching libicu package installed -
# one less thing to get onto an offline server. Safe here since the app
# deals in IPs/VLANs/technical data, not locale-sensitive formatting.
dotnet publish "$API_CSPROJ" \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=false \
  -p:InvariantGlobalization=true \
  -o "$OUT/api"

echo "==> Building frontend -> $OUT/client"
(cd client && npm ci && npm run build)
mkdir -p "$OUT"
cp -r client/dist "$OUT/client"

echo "$TAG" > deploy/dist/LATEST
echo "==> Release ready: $OUT (tag: $TAG)"
