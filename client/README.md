# Remote Shell Interrupt — Web Client

React 19 + TypeScript + Vite SPA for the Remote Shell Interrupt backend
(ASP.NET Core API in `../src`). Built with MUI, TanStack Query,
Zustand, React Hook Form + Zod, Axios and React Router.

## Getting started

Run the commands below from `client/`. Supported Node.js versions match
`package.json`: `^20.19.0`, `^22.13.0`, or `>=24`.

```bash
npm ci
npm run dev      # dev server on http://localhost:3000
npm run typecheck
npm run build    # typecheck + production build
npm run lint     # ESLint
npm test         # regression tests
npm run test:watch    # rerun affected tests while editing
npm run test:coverage # V8 coverage, including HTML report
npm run format   # apply the repository formatting policy
npm run check    # formatting + lint + typecheck + tests + production build
npm run preview  # serve the production build
```

The API base URL is read from `VITE_API_URL`. Copy `.env.example` to `.env`
(or create `.env.development` / `.env.production`) and point it at the backend
port — see `../src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API/Properties/launchSettings.json`.
The client calls the versioned backend routes under `/api/v1`.

`VITE_API_URL` is the API origin/base URL (for example `http://localhost:5000`),
not a URL ending in `/api/v1`. An unset value means same-origin requests. Vite
reads this setting during development/build: restart the dev server after
changing it and rebuild to change a production bundle. The dev server does not
proxy API requests automatically.

When the API runs on a separate origin, configure its `Cors:AllowedOrigins`
(for example `Cors__AllowedOrigins__0=http://localhost:3000`). The API allows
credentials for configured origins. Use a consistent hostname during local
development; `localhost` and `127.0.0.1` are different browser origins and cookie
hosts.

## Authentication and access

The sign-in page calls `/api/v1/Auth/CookieLogin`; Axios sends requests with
`withCredentials: true`. The session credential is an HttpOnly cookie. Zustand
holds the current user and authentication state, and `rsi.auth.user` in
localStorage caches only the non-secret profile. Restoring that profile is
optimistic; the backend still authorizes each request.

Logout uses `/api/v1/Auth/CookieLogout`, clears the local session and query cache,
and returns to sign-in. Protected-request HTTP 401 responses also clear the
session. The SPA does not store JWT access or refresh tokens.

Authenticated users can access VLAN search, devices, clients, VLANs and tariff
plans. Gates, workflows, user management, account registration and administration
require Admin. Registration provisions another account without replacing the
administrator's session. Route guards and navigation reflect these roles;
authorization is also enforced by the API.

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
    ├── auth/             # Cookie-session profile store and authentication API
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
- **HTTP concerns** — one transport owns base URL, timeout, cookie credentials, request
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

## Engineering workspace

- The compact navigation rail keeps all role-authorized destinations; narrow
  screens use the menu drawer.
- VLAN searches use `?vlan=120` in the URL. Opening a shared link restores the
  search; browser history returns to earlier VLANs. Reset removes the search.
- Search and device detail pages share an interface table with local text/status
  filters, adjustable row density and a closeable detail panel. Counts describe
  loaded records. Statuses come from collected data, not live monitoring.
- Interface details retain properties, VLANs, learned MAC addresses, aggregation
  members and optional ARP/network data. IPs and learned MACs can be copied.
- Device and customer directories default to tables; existing card/list views,
  server filters and pagination remain available. Customer sorting is retained;
  device ordering continues to use the existing host sort.
- Customer details group the existing sections into Overview, Network & plan,
  Contacts and Notes & history.
- Workflow editing offers Focus canvas and a More actions section for import,
  export, copying and lifecycle operations. Draft/read-only rules, confirmation
  dialogs, unsaved-change guards and execution state still apply.

Interface filtering applies to the records already loaded for the current page
or VLAN search. It does not issue a new network-wide search. The detail panel
appears beside the interface table on large screens and below it on smaller
screens. Only the VLAN query is restored from its URL; local interface filters,
selection, density and directory view modes are not persisted across navigation.

In the workflow editor, the node palette moves to the left on extra-wide screens
and stays above the canvas on smaller screens. Focus canvas hides the palette and
inspector without resetting the graph. Execution stays mounted when switching
editor tabs or editing the graph, preserving entered parameters and running
requests. Cancelling a request does not undo devices already saved by a workflow.

### Main routes

| Route                                                              | Screen                          | Access         |
| ------------------------------------------------------------------ | ------------------------------- | -------------- |
| `/mainPage?vlan=120`                                               | VLAN search                     | Signed-in user |
| `/networkDevices`, `/networkDevices/:id`                           | Device directory and interfaces | Signed-in user |
| `/clients`, `/clients/:id`                                         | Customer directory and details  | Signed-in user |
| `/sprVlans`, `/tfPlans`                                            | VLAN and tariff directories     | Signed-in user |
| `/gates`, `/createGate`, `/gates/:id`                              | Gate directory and forms        | Admin          |
| `/admin`                                                           | Synchronization and cleanup     | Admin          |
| `/admin/users`, `/register`                                        | Accounts and provisioning       | Admin          |
| `/admin/workflows`, `/admin/workflows/new`, `/admin/workflows/:id` | Workflow catalog and editor     | Admin          |

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
- Engineering workspace: interface selection, diagnostic tabs (MAC, aggregation,
  ARP and networks), local filtering and empty-result recovery, unknown statuses,
  close/reopen behavior, clipboard errors and restoring/resetting VLAN URLs.
- Gates: create/edit validation, pending submission, failed saves, cancellation,
  loading existing values and query-cache invalidation. Bulk-delete tests ensure
  every page is read before deletion begins and errors stop the operation.
- Devices and clients: loading/error/empty states, record links, pagination,
  filters, sorting and view selection. Detail tests cover optional customer data,
  contacts/history, port speed units, empty diagnostics, and independent expansion
  of MAC tables and aggregated interfaces. Filter tests exercise submission,
  normalization, pagination reset and recovery from an empty result.
- Account administration: self-account protections, role changes, deletion
  confirmation, pending operations, rollback after failed activation changes,
  profile validation and retry. Registration tests cover administrator-only
  access, password confirmation, selected roles, preserving the administrator's
  session and refreshing the user directory.
- Administrative operations: cancellation of bulk-delete confirmations, selecting
  the correct deletion, blocking concurrent operations, error recovery and
  invalidating clients, tariffs and VLANs after synchronization.
- Workflows: catalog search and retry, graph import/validation, ID remapping,
  execution errors and cancellation contracts. Designer-state tests exercise
  undo/redo, read-only and busy states, unique Start nodes, insertion, reconnection,
  grid snapping and resetting the saved baseline.
- Editor layout: Focus canvas preserves the graph, More actions retains access
  to import/export/copy, and graph edits preserve execution-panel state.
- Workflow execution UI: saved/dirty guards, JSON input validation, request
  payloads, busy-state locking, trace and output rendering, application and
  transport failures, retry, cancellation, unmount cleanup and device-cache
  invalidation.
- HTTP transport: cookies, normalized errors, handling 401 responses, cancellation
  and tracking concurrent requests. A local Axios adapter keeps these tests offline.
- Additional screens: tariff/VLAN directories, gates and users lists, home/guest
  navigation, error pages and the API-error demonstration screen.

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

Use the current Vitest summary and generated coverage report for test counts and
coverage percentages; these values are not fixed documentation targets.

jsdom and jest-dom are pinned to versions compatible with the project's supported
Node.js versions. When updating them, check their engine requirements before
raising the versions.

Testing references: [React Testing Library setup](https://testing-library.com/docs/react-testing-library/setup/)
and [Vitest coverage](https://vitest.dev/guide/coverage).
