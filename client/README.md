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
npm run test:watch    # rerun affected tests while editing
npm run test:coverage # V8 coverage, including HTML report
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
│   ├── <Area>/
│   │   ├── api/          # Domain API, query keys and TanStack Query hooks
│   │   ├── List/         # List UI and local page state
│   │   └── Detail/       # Detail UI
│   └── Workflows/        # Canvas editor for workflow graphs (larger feature:
│                         # api/, domain/workflow/ model + graph helpers,
│                         # designer/{components,state}/ for the canvas)
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

## Frontend tests

Tests use Vitest. Component suites opt into jsdom with a file-level directive;
pure helper and domain tests run in Node. React Testing Library and user-event
exercise forms, buttons and navigation through accessible labels and roles.

The suites cover:

- Authentication: form validation, password visibility, pending and failed login,
  retry, invalid server responses, redirect restoration, role guards, logout and
  clearing session data. Store tests also check persistence and cross-tab events.
- Navigation: role-based destinations and active links on nested routes.
- VLAN search: boundary validation, keyboard submission, loading, empty results,
  API errors, recovery, retaining input and resetting results.
- Gates: create/edit validation, pending submission, failed saves, cancellation,
  loading existing values and query-cache invalidation. Bulk-delete tests ensure
  every page is read before deletion begins and errors stop the operation.
- Devices and clients: loading/error/empty states, record links, pagination,
  filters, sorting and view selection.
- Workflows: catalog search and retry, graph import/validation, ID remapping,
  execution errors and cancellation contracts. Designer-state tests exercise
  undo/redo, read-only and busy states, unique Start nodes, insertion, reconnection,
  grid snapping and resetting the saved baseline.
- HTTP transport: cookies, normalized errors, handling 401 responses, cancellation
  and tracking concurrent requests. A local Axios adapter keeps these tests offline.

`tests/renderApp.tsx` creates an isolated query cache, theme and memory router for
each render. UI tests mock the feature API boundary while keeping real forms,
validation, query hooks and routing. They do not contact the backend or require a
database. These are component/integration tests, not browser end-to-end or visual
regression tests.

`npm run typecheck` checks production and test TypeScript; `npm run check` includes
it. Run `npm run test:coverage` and open `coverage/index.html` for line and branch
coverage. The report includes untested source files, so its total reflects the
whole frontend (excluding type-only DTOs and the entry point), not just tested
modules. Reports are ignored by Git. No claim of complete coverage is implied.

jsdom and jest-dom are pinned to versions compatible with the project's supported
Node.js versions. When updating them, check their engine requirements before
raising the versions.

Testing references: [React Testing Library setup](https://testing-library.com/docs/react-testing-library/setup/)
and [Vitest coverage](https://vitest.dev/guide/coverage).
