# syntax=docker/dockerfile:1

# --- Build ---
FROM node:22-alpine AS build
WORKDIR /src

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# No VITE_API_URL: baked in as same-origin, so the built app calls relative
# /api/v1/... paths — see nginx.conf, which proxies those to the api service.
RUN npm run build

# --- Runtime ---
FROM nginx:1.27-alpine AS runtime

COPY --from=build /src/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=5 \
    CMD wget -qO- http://localhost/ >/dev/null || exit 1
