# Remote Shell Interrupt — Web Client

React 19 + TypeScript + Vite SPA for the Remote Shell Interrupt backend
(ASP.NET Core API in `../src`). Built with MUI, TanStack Query,
React Hook Form + Zod, Axios and React Router.

## Getting started

Requires Node.js 20.19+, 22.13+, or 24+.

```bash
npm install
npm run dev      # dev server on http://localhost:3000
npm run typecheck
npm run build    # typecheck + production build
npm run lint     # ESLint
npm test         # regression tests
npm run format   # apply the repository formatting policy
npm run check    # formatting + lint + tests + production build
npm run preview  # serve the production build
```

The API base URL is read from `VITE_API_URL`. Copy `.env.example` to `.env`
(or create `.env.development` / `.env.production`) and point it at the backend
port — see `../src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API/Properties/launchSettings.json`.
The client calls the versioned backend routes under `/api/v1`.

## Docker

`docker compose up --build` from the repo root builds this client (see
`Dockerfile`, `nginx.conf`) alongside the API and Postgres — no Node install
needed. The image is a static production build (no `VITE_API_URL` baked in,
so it defaults to same-origin) served by nginx, which reverse-proxies
`/api/` and `/health` to the API container. That means the browser only ever
talks to one origin; the backend's CORS policy is irrelevant to this
deployment shape. See the root README's "Or with Docker Compose" section.

## Architecture

```
src/
├── main.tsx              # Minimal application bootstrap
├── app/
│   ├── AppProviders.tsx  # Root composition: errors, query, router, toasts
│   ├── layout/           # App shell: NavBar, global busy indicator, styles
│   ├── router/           # Route table, guards and centralized paths
│   └── shared/           # Cross-feature presentational components
├── config/               # Validated environment and backend route config
├── features/             # Feature-sliced domain modules
│   └── <Area>/
│       ├── api/          # Domain API, query keys and TanStack Query hooks
│       ├── List/         # List UI and local page state
│       └── Detail/       # Detail UI
└── lib/
    ├── api/              # HTTP transport, normalized errors, shared API helpers
    ├── auth/             # Session store, JWT parsing and authentication API
    ├── constants/        # Shared immutable defaults
    ├── schemas/          # Zod validation schemas (react-hook-form resolvers)
    ├── stores/           # Cross-cutting UI state
    ├── types/            # API DTO types, mirroring the backend camelCase JSON
    └── utils.ts          # Formatting helpers
```

Conventions:

- **Dependency direction** — UI calls its feature's query hooks; query hooks call
  the feature API; feature APIs call the shared HTTP transport. Shared `lib` code
  never imports feature UI.
- **Query cache** — every domain owns a typed query-key factory. Mutations use
  those factories for explicit cross-domain invalidation; no URL-derived keys or
  duplicated string literals are used.
- **HTTP concerns** — one transport owns base URL, timeout, bearer token, request
  activity and 401 handling. Domain APIs own endpoints and response types;
  components receive normalized `ApiError` instances.
- **Routing** — route declarations, links and imperative navigation all use
  `app/router/paths.ts`, including encoded dynamic segments.
- **Failures** — expected API failures stay local to the requesting screen;
  application and router boundaries handle unexpected render/chunk failures,
  both using one observability integration point.
- **Quality gate** — `npm run check` is the single local/CI command for format,
  lint, regression tests, type checking and the optimized production bundle.
- **Pagination** — list endpoints return the page body plus an `X-Pagination`
  header (PascalCase JSON, produced by a plain `JsonSerializer.Serialize` on
  the backend). `fetchPaged` merges both into a `PagedResponse<T>`.
- **Error payloads** — the backend middleware also serializes errors with
  plain `JsonSerializer.Serialize`, so `ApiErrorResponse` is PascalCase while
  DTO bodies are camelCase. Don't "normalize" one without the other.
- **DTO casing** — types in `lib/types` mirror the serialized JSON exactly;
  MUI components and forms use camelCase.
