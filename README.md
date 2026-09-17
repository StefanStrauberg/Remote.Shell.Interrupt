# 🌐 Remote.Shell.Interrupt

**A network infrastructure monitoring platform:** SNMP-based data collection from routers, and billing system synchronization, exposed as a role-based REST API.

- **Backend** — .NET 9, Clean Architecture, CQRS
- **Databases** — PostgreSQL (primary) + MySQL (billing gateway, read-only)
- **Tests** — xUnit unit and integration suites (real PostgreSQL/MySQL via Testcontainers); Vitest + React Testing Library for frontend unit and component/integration tests
- **Frontend** — React 19 + TypeScript + Vite SPA (see [Frontend](#frontend) below)

---

## 🧱 Architecture

```text
Remote.Shell.Interrupt/
├── Remote.Shell.Interrupt.sln
├── docker-compose.yml                  # API + PostgreSQL + web client, auto-migrates on startup (see Quick Start)
├── Dockerfile                          # Builds the API image (used by docker-compose.yml)
├── src/Remote.Shell.Interrupt.Storehouse/
│   ├── Core/
│   │   ├── ...Storehouse.Domain        # Domain entities
│   │   └── ...Storehouse.Application   # CQRS, DTOs, validation, contracts
│   ├── Infrastructure/                 # SNMP, workflow engine, logger, specifications, filter parser
│   ├── Persistence/                    # EF Core (PostgreSQL), Identity, Dapper (MySQL)
│   └── Remote.Shell.Interrupt.Storehouse.API/  # ASP.NET Core 9 — API host
├── client/                             # React 19 + TypeScript + Vite SPA (see Frontend, client/README.md)
├── scripts/                            # Dev utilities (see Test data export/import below)
├── Tests/                              # xUnit unit tests (mocks/InMemory/SQLite, no external services)
└── Tests.Integration/                  # xUnit tests against real PostgreSQL/MySQL (Testcontainers, needs Docker)
```

---

## 🛠️ Technologies

### Backend

- **ASP.NET Core 9** — REST API, versioned under `/api/v1`
- **EF Core 9 + Npgsql** — PostgreSQL data access
- **ASP.NET Core Identity** — authentication (JWT + Cookie), roles
- **MediatR** — CQRS pipeline (validation + logging)
- **AutoMapper** — DTO mapping
- **FluentValidation** — command validation
- **Serilog** — structured request logging (console + files), correlation ID per request
- **SharpSnmpLib** — SNMP v2c
- **Dapper + MySql.Data** — read-only gateway to the remote billing database
- **Microsoft.Extensions.Diagnostics.HealthChecks** — liveness/readiness probes for PostgreSQL and the MySQL billing connection

> **Why two data-access technologies?** PostgreSQL is owned by this application (full schema knowledge, EF Core migrations). The MySQL billing database is owned by a third party: its full schema is unknown, this app is only permitted to read specific columns from specific tables, and it must never write to it. EF Core wants to fully model and evolve a schema it owns, which doesn't fit that constraint — Dapper's "run this SQL, map these columns" model does. The MySQL connection additionally issues `SET SESSION TRANSACTION READ ONLY` on open, so even a future coding mistake that tried to write would be rejected by the database itself, not just by code review.

### Frontend

A React 19 + TypeScript + Vite SPA under [`client/`](client/) — MUI, TanStack Query, Zustand, React Hook Form + Zod, Axios, React Router. Feature-sliced (`src/features/<Area>/{api,List,Detail}`), with its own [README](client/README.md) covering setup, architecture, UI behavior and tests. The SPA calls the backend's versioned `/api/v1` routes and signs in through `Auth/CookieLogin` using an HttpOnly session cookie. Only the non-secret user profile is cached in localStorage; browser requests do not carry a JavaScript-managed bearer token.

Run it standalone against a locally-running API with Node.js matching `client/package.json` (`^20.19.0`, `^22.13.0`, or `>=24`):

```bash
cd client
npm ci
npm run dev      # http://localhost:3000, VITE_API_URL from .env (copy .env.example)
npm run check    # formatting, ESLint, Vitest, TypeScript and production build
```

Or let Docker Compose build and serve it (see below) — no Node install needed.

The UI is organized as an **engineering workspace**:

- Compact navigation with role-based destinations and a menu drawer on narrow screens.
- VLAN search with shareable URLs, for example `/mainPage?vlan=120`, and combined customer/device results.
- A grouped interface table with local text/status filters, compact rows and a closeable detail panel. Details include VLANs, MAC addresses, aggregation members and optional ARP/network data; IPs and learned MACs can be copied. Statuses describe collected SNMP data, not a live monitoring feed.
- Device and customer directories default to tables while retaining card views, server-side filters and pagination. Customers also retain list view and sorting.
- Customer details are grouped into Overview, Network & plan, Contacts, and Notes & history.
- Tariff plans, VLANs, gates, billing synchronization, account provisioning and user administration remain available according to the user's role.
- The visual workflow designer (Admin → Workflows) supports drag/drop, undo/redo, validation, JSON import/export, draft copies and lifecycle actions. **Focus canvas** hides editing panels; **More actions** groups less frequent commands. The execution panel runs a saved graph and displays its trace after completion. Read-only states, unsaved-change protection and request cancellation are retained.
- Light/dark theme toggle (top-right, on any authenticated page) — defaults to the OS color-scheme preference, persisted per-browser in `localStorage`.

Logout clears the local session and cached business data. A protected API request returning HTTP 401 clears the expired session and returns the user to sign-in. JWT login, refresh and revocation remain separate API capabilities for API clients; the SPA uses the cookie flow.

---

## 🚀 Quick Start

### Requirements

- **.NET 9 SDK**
- **PostgreSQL 14+**
- MySQL — only for billing synchronization

### Run the API

```bash
dotnet run --project src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API
```

On startup the API **automatically** applies pending EF Core migrations (creating the full schema from scratch on a fresh/empty database, e.g. a newly deployed container) and creates the roles and the administrator account. By default it listens on `http://localhost:5000`.

### Or with Docker Compose

```bash
docker compose up --build
```

Builds the API and web client images and starts them alongside a PostgreSQL container. The API waits for Postgres to become healthy, then applies migrations and seeds identity on startup exactly as above; the client waits for the API to become healthy. Once everything is up:

| Service  | URL                                                 |
| -------- | --------------------------------------------------- |
| Web UI   | `http://localhost:3000`                             |
| API      | `http://localhost:5000` (`/swagger` in Development) |
| Postgres | `localhost:5432`                                    |

The client container is nginx serving the production build, reverse-proxying `/api/` and `/health` to the API container (see `client/nginx.conf`) — the browser only ever talks to one origin, so the backend's CORS policy never comes into play for this setup. If you instead run the client with `npm run dev` against a differently-hosted API, or serve it from a different origin than the API in a real deployment, set `Cors__AllowedOrigins__0` (etc.) on the API so the browser is actually allowed to call it cross-origin.

Works out of the box with the same dev credentials as above (`JwtSettings:Key`, `IdentitySeed:AdminPassword`, etc. all have defaults baked into `docker-compose.yml`). To override them — e.g. a real JWT key and admin password for anything beyond local/dev use — copy [`.env.example`](.env.example) to `.env` and edit it; `docker compose` picks it up automatically.

The MySQL billing connection (`ConnectionStrings__DefaultConnection2`) is **not** part of this Compose setup — it stays unset, so `/health/ready` reports the `mysql-billing` check unhealthy and billing-sync endpoints won't work until you supply a connection string pointing at your own MySQL instance. For Compose, add it to the API service's `environment` section, directly or through a Compose override; adding a variable to `.env` alone does not pass it into the container. Everything else (auth, gates, network devices, dashboards) works fully against just Postgres.

In Development, Swagger UI is available at `http://localhost:5000/swagger` — use it to call `POST /api/v1/Auth/Login` (see below) and then "Authorize" with the returned token to exercise protected endpoints from the browser.

### Sign in (Development configuration)

```bash
curl -X POST http://localhost:5000/api/v1/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@localhost.local","password":"Admin#Dev-Only-2025"}'
```

| Parameter | Value                   |
| --------- | ----------------------- |
| Email     | `admin@localhost.local` |
| Password  | `Admin#Dev-Only-2025`   |

### Postman collection

[`Remote.Shell.postman_collection.json`](Remote.Shell.postman_collection.json) covers every endpoint. Import it, run **Auth > Login** once — its test script saves the token into a collection variable, so every other request authenticates automatically. See the collection's own description (visible in Postman) for details on filter query parameters and the SNMP simulator variables.

---

## 🔐 Authentication & Roles

- **JWT Bearer** — for API requests (`Authorization: Bearer <token>`)
- **HttpOnly Cookie** — for browser sessions
- **Global authorization policy** — every endpoint is secured by default

### Authorization API

| Method | Route                       | Access                                    |
| ------ | --------------------------- | ----------------------------------------- |
| POST   | `/api/v1/Auth/Login`        | anonymous                                 |
| POST   | `/api/v1/Auth/RefreshToken` | anonymous (refresh token required)        |
| POST   | `/api/v1/Auth/RevokeToken`  | anonymous (refresh token in request body) |
| POST   | `/api/v1/Auth/Register`     | Admin                                     |
| POST   | `/api/v1/Auth/CookieLogin`  | anonymous                                 |
| POST   | `/api/v1/Auth/CookieLogout` | authenticated                             |

### Access Matrix

| Capability                                      | Admin | User |
| ----------------------------------------------- | :---: | :--: |
| Dashboards, VLAN search, clients, tariff plans  |  ✅   |  ✅  |
| Viewing network devices                         |  ✅   |  ✅  |
| Creating / deleting network devices             |  ✅   |  ❌  |
| Gates: view / create / update / delete          |  ✅   |  ❌  |
| Billing sync and cleanup                        |  ✅   |  ❌  |
| Registering users                               |  ✅   |  ❌  |
| Managing user profiles, roles and active status |  ✅   |  ❌  |
| SNMP Get / Walk                                 |  ✅   |  ❌  |
| Workflow graphs: create / update / delete / run |  ✅   |  ❌  |

---

## 🏥 Health Checks

For use as liveness/readiness probes behind a load balancer or orchestrator. All three are anonymous.

| Route           | Checks                                | Use as          |
| --------------- | ------------------------------------- | --------------- |
| `/health/live`  | none — process is responding          | liveness probe  |
| `/health/ready` | PostgreSQL + MySQL billing connection | readiness probe |
| `/health`       | everything                            | manual check    |

---

## ✨ Features

### Completed

- 📡 **SNMP router polling** — ports, ARP, MAC tables, VLANs (Juniper / Huawei / Extreme / Cisco / FortiGate)
- 👥 **Billing clients** — local database synchronization with the remote billing system and fast search
- 🔎 **Compound VLAN search** — clients and network devices in a single query
- 🖥️ **Dashboards** — filters, sorting, server-side pagination
- 🚪 **Gate management** — create, update, delete with duplicate checks
- 🛡️ **Admin panel** — billing data refresh and cleanup
- 🖥️ **Web frontend** — responsive engineering workspace with protected routes, searchable interface tables, diagnostic detail panels, customer tabs, gate forms, user management, and a visual workflow designer
- 🔐 **Role-based access** — Admin / User with protected routes and API
- 🧬 **Workflow engine** — node/edge graphs (`Start`/`End`/`Decision`/`Join`/`SetVariable`/`SnmpGet`/`SnmpWalk`/`Script`/`SaveNetworkDevice`) routed by priority/condition matching, run against a device over SNMP; `Script` nodes execute sandboxed JavaScript (Jint, `function execute(input, context)` contract) for vendor-specific data transforms, with `console.log` output captured per step; `Draft → Published → Archived` lifecycle (a Published graph is immutable); full CRUD + Publish/Archive via `WorkflowsController`, editable end-to-end from the SPA's canvas designer (Admin → Workflows). The vendor-specific SNMP discovery logic (Juniper/Huawei/Extreme port, VLAN and link-aggregation parsing) that used to be a ~900-line hand-coded handler is now the seeded "Network device discovery" workflow itself — `POST /NetworkDevices/CreateNetworkDevice` just runs it
- 🧵 **Correlation ID** — per-request ID threaded through Serilog's log context (controller → MediatR → repositories) and echoed back on the response
- 🏥 **Health checks** — `/health/live`, `/health/ready`, `/health` (see above)
- 🔢 **API versioning** — all routes under `/api/v1`
- 🐳 **Docker Compose** — web client + API + PostgreSQL, migrations and identity seeding run automatically on startup (see [Quick Start](#-quick-start))

### Planned

- 🧪 CI/CD

---

## ⚙️ Configuration

| Variable                                | Description                                                                           |
| --------------------------------------- | ------------------------------------------------------------------------------------- |
| `ConnectionStrings__DefaultConnection`  | PostgreSQL connection string                                                          |
| `ConnectionStrings__DefaultConnection2` | remote MySQL billing database                                                         |
| `JwtSettings__Key`                      | JWT signing key (min. 32 characters, required)                                        |
| `IdentitySeed__AdminEmail`              | default administrator email                                                           |
| `IdentitySeed__AdminPassword`           | administrator password (empty — do not create)                                        |
| `Database__AutoMigrate`                 | apply EF Core migrations on startup (default `true`)                                  |
| `Cors__AllowedOrigins__0`               | allowed browser origin when serving the client separately from the API                |
| `VITE_API_URL`                          | frontend API base URL, read by Vite during development/build; unset means same-origin |

Secrets are provided via user-secrets or environment variables:

```bash
cd src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Key" "signing-key-min-32-characters"
dotnet user-secrets set "IdentitySeed:AdminPassword" "a-strong-password"
```

---

## 🗄️ Database

PostgreSQL schema is managed with real **EF Core Migrations**, stored in `src/Remote.Shell.Interrupt.Storehouse/Persistence/Remote.Shell.Interrupt.Storehouse.Dapper.Persistence/Migrations/`. On startup the API calls `Database.MigrateAsync()`, which:

- creates the entire schema on a fresh/empty database (a newly deployed container needs no manual setup), and
- applies only the migrations not yet recorded in `__EFMigrationsHistory` on an existing one.

Several replicas starting at once don't race each other into `MigrateAsync()` concurrently: the call is wrapped in a Postgres advisory lock, so only one replica actually migrates while the rest block briefly and then see the schema already at the target version.

Set `Database__AutoMigrate` to `false` to opt out of this entirely — e.g. a deployment that runs `dotnet ef database update` once from a release pipeline and wants the application itself to never touch the schema. Defaults to `true` (unset = on) in every environment, matching the Quick Start flow above; with it disabled, the host still starts even if the schema is out of date, and only fails once a query hits a table/column a pending migration would have added.

MySQL (`ConnectionStrings__DefaultConnection2`) is an externally-owned, read-only billing database — it is never migrated by this application.

This repo pins the [`dotnet-ef`](https://www.nuget.org/packages/dotnet-ef) CLI as a local tool (`.config/dotnet-tools.json`), so no global install is needed:

```bash
dotnet tool restore
```

After changing an entity or its `IEntityTypeConfiguration<T>`, add a migration:

```bash
dotnet ef migrations add <DescriptiveName> \
  --project src/Remote.Shell.Interrupt.Storehouse/Persistence/Remote.Shell.Interrupt.Storehouse.Dapper.Persistence \
  --startup-project src/Remote.Shell.Interrupt.Storehouse/Persistence/Remote.Shell.Interrupt.Storehouse.Dapper.Persistence \
  --output-dir Migrations
```

The Persistence project doubles as its own startup project via `ApplicationDbContextFactory` (an `IDesignTimeDbContextFactory<ApplicationDbContext>`), so `migrations add` needs no database connection and no runtime secrets (JWT key, etc.) — those only matter for actually running the API. Commit the generated migration, and the next application start (or a manual `dotnet ef database update` with `ConnectionStrings__DefaultConnection` set) applies it.

---

## 🧰 Test data export/import

`scripts/export_test_data.py` dumps `Clients` / `TfPlans` / `SPRVlans` (optionally `CODs`) rows out of a running PostgreSQL container as plain-SQL, data-only files — handy for seeding a separate local/test database with real-shaped data without a live billing sync.

```bash
python scripts/export_test_data.py                        # from this repo's own `docker compose` postgres service
python scripts/export_test_data.py --container my-test-db --db testing --user admin --password secret
```

The `--container` form talks to any standalone `docker run` PostgreSQL container (bypassing `docker compose` entirely) — use it when the source data lives outside this repo's Compose stack. Dumps land in `db_export/` (**gitignored** — they contain real customer data, never commit them) using `--data-only --disable-triggers`, ready to pipe into another Postgres container:

```bash
Get-Content db_export/Clients_*.sql | docker exec -i <target-container> psql -U postgres -d remote_shell_interrupt
```

Run `python scripts/export_test_data.py --help` (or read the script's own docstring) for every flag, including `--tables`, `--combined` and `--readable`.

---

## 🧪 Tests

```bash
dotnet test Tests/Tests.csproj
```

Unit tests across Domain, Application, Infrastructure, Persistence, and API use mocks, EF Core InMemory, and SQLite standing in for MySQL. No external services required. Use the test runner's summary for the current test count.

### Frontend checks

```bash
cd client
npm ci
npm run check
npm run test:coverage
```

`npm run check` runs Prettier, ESLint, TypeScript checks for application and tests, the complete Vitest suite, and the Vite production build. It stops at the first failing stage. Run `npm run format` to apply formatting fixes, then rerun `npm run check`.

The frontend tests cover authentication and roles, registration and account management, VLAN search, directories and detail pages, administrative operations, workflow editing/execution, and HTTP/query-cache behavior. Engineering-workspace tests exercise interface selection, diagnostic tabs, status filtering, clipboard success/failure, and VLAN URL restoration. UI tests mock API boundaries and do not require a running backend or database.

`npm run test:coverage` generates an HTML report at `client/coverage/index.html` and a machine-readable summary. `npm run test:watch` reruns tests during development. These are unit and component/integration suites; they do not replace browser end-to-end checks against a deployed backend. See [frontend testing details](client/README.md#frontend-tests).

### Integration tests

```bash
dotnet test Tests.Integration/Tests.Integration.csproj
```

These tests boot the real API pipeline (the same startup sequence as `Program.cs` — migrations, identity seeding, the full middleware pipeline) against **ephemeral PostgreSQL and MySQL containers** started via [Testcontainers](https://testcontainers.com/) — entirely separate from any database already running on the machine, torn down after the run. Requires **Docker** to be running; otherwise these fail to start the containers. Kept in a separate project (and out of plain `dotnet test` at the repo root) so the fast unit suite stays Docker-free.

Covers what the unit suite structurally cannot: real EF Core migrations actually applying to Postgres, `ILIKE` filtering executing against a real Npgsql provider, the JWT/cookie/role-authorization pipeline end-to-end over real HTTP, health checks against live dependencies, `SET SESSION TRANSACTION READ ONLY` genuinely rejecting a write on the MySQL connection, and a full workflow round-trip (create → update → execute with a real Jint script → publish → archive → delete).

---

## 📡 SNMP Simulator (manual testing without real hardware)

A standalone SNMP v2c server that replays a captured `snmpwalk` dump — a stand-in router for manually exercising `POST /api/v1/NetworkDevices`, `SNMPExecutor/Get`, `SNMPExecutor/Walk`, etc. without needing a real device on hand. It now lives in its own repository: **[StefanStrauberg/SnmpSimulator](https://github.com/StefanStrauberg/SnmpSimulator)** — clone it separately; see its own README for setup, running it, and capturing a dump from a real router.

Two ways to run it against this project:

- **Quick local check** — run it directly on `127.0.0.1:1161` (its default) and point ad-hoc `snmpget`/`snmpwalk` calls at that address. Fastest way to validate a dump file, but the API only ever talks SNMP on port 161 to a device's own IP (see `SNMPCommandExecutor.cs`), so this mode doesn't exercise the API itself.
- **Through the API, at a real router's IP** — bring this project's stack up first (`docker compose up -d`) so the `remote-shell-interrupt-net` Docker network exists, then start the simulator from its own repo attached to that same network (`external: true`), pinned to a static IP on `192.168.101.0/24` (e.g. `192.168.101.8`) — same subnet the `api` container joins here. The API then talks to it exactly as it would a real router — same port, same IP shape, same code path — which is the closest thing to testing production behavior without real hardware. This only works because the machine running Docker isn't simultaneously on a real `192.168.101.0/24` network — see the comment above the `app-net` network in `docker-compose.yml`.

---

## 📄 License

See [LICENSE.md](LICENSE.md).
