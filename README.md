# 🌐 Remote.Shell.Interrupt

**A network infrastructure monitoring platform:** SNMP-based data collection from routers, and billing system synchronization, exposed as a role-based REST API.

- **Backend** — .NET 9, Clean Architecture, CQRS
- **Databases** — PostgreSQL (primary) + MySQL (billing gateway, read-only)
- **Tests** — xUnit, 514 unit tests + 20 integration tests (real PostgreSQL/MySQL via Testcontainers)
- **Frontend** — not currently in this repository (see [Frontend](#frontend) below)

---

## 🧱 Architecture

```text
Remote.Shell.Interrupt/
├── Remote.Shell.Interrupt.sln
├── docker-compose.yml                  # API + PostgreSQL, auto-migrates on startup (see Quick Start)
├── docker-compose.snmp.yml             # Simulated routers on the API's network (see SNMP Simulator)
├── Dockerfile                          # Builds the API image (used by docker-compose.yml)
├── src/Remote.Shell.Interrupt.Storehouse/
│   ├── Core/
│   │   ├── ...Storehouse.Domain        # Domain entities
│   │   └── ...Storehouse.Application   # CQRS, DTOs, validation, contracts
│   ├── Infrastructure/                 # SNMP, logger, specifications, filter parser
│   ├── Persistence/                    # EF Core (PostgreSQL), Identity, Dapper (MySQL)
│   └── Remote.Shell.Interrupt.Storehouse.API/  # ASP.NET Core 9 — API host
├── SnmpSimulator/                      # Standalone SNMP v2c dump-replay server for local testing (see SNMP Simulator, SnmpSimulator/README.md)
├── Tests/                              # xUnit — 514 unit tests (mocks/InMemory/SQLite, no external services)
└── Tests.Integration/                  # xUnit — 20 tests against real PostgreSQL/MySQL (Testcontainers, needs Docker)
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

A React 19 + TypeScript + Vite SPA (MUI, TanStack Query, Zustand, React Hook Form + Zod) previously lived under `client/` but has been removed from this repository. The API has no bundled UI at the moment — interact with it via Swagger (see below) or any HTTP client.

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

Builds the API image and starts it alongside a PostgreSQL container. The API waits for Postgres to become healthy, then applies migrations and seeds identity on startup exactly as above — no manual setup needed. Also listens on `http://localhost:5000` by default.

Works out of the box with the same dev credentials as above (`JwtSettings:Key`, `IdentitySeed:AdminPassword`, etc. all have defaults baked into `docker-compose.yml`). To override them — e.g. a real JWT key and admin password for anything beyond local/dev use — copy [`.env.example`](.env.example) to `.env` and edit it; `docker compose` picks it up automatically.

The MySQL billing connection (`ConnectionStrings__DefaultConnection2`) is **not** part of this Compose setup — it stays unset, so `/health/ready` reports the `mysql-billing` check unhealthy and billing-sync endpoints won't work until you set that connection string yourself (e.g. via `.env`, pointing at your own MySQL instance). Everything else (auth, gates, network devices, dashboards) works fully against just Postgres.

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

---

## 🔐 Authentication & Roles

- **JWT Bearer** — for API requests (`Authorization: Bearer <token>`)
- **HttpOnly Cookie** — for browser sessions
- **Global authorization policy** — every endpoint is secured by default

### Authorization API

| Method | Route                        | Access        |
| ------ | ----------------------------- | ------------- |
| POST   | `/api/v1/Auth/Login`          | anonymous     |
| POST   | `/api/v1/Auth/Register`       | Admin         |
| POST   | `/api/v1/Auth/CookieLogin`    | anonymous     |
| POST   | `/api/v1/Auth/CookieLogout`   | authenticated |

### Access Matrix

| Capability                                     | Admin | User |
| ---------------------------------------------- | :---: | :--: |
| Dashboards, VLAN search, clients, tariff plans |  ✅   |  ✅  |
| Viewing network devices                        |  ✅   |  ✅  |
| Creating / deleting network devices            |  ✅   |  ❌  |
| Gates: view / create / update / delete         |  ✅   |  ❌  |
| Billing sync and cleanup                       |  ✅   |  ❌  |
| Registering users                              |  ✅   |  ❌  |
| SNMP Get / Walk                                |  ✅   |  ❌  |

---

## 🏥 Health Checks

For use as liveness/readiness probes behind a load balancer or orchestrator. All three are anonymous.

| Route            | Checks                              | Use as             |
| ----------------- | ------------------------------------ | ------------------- |
| `/health/live`    | none — process is responding         | liveness probe       |
| `/health/ready`   | PostgreSQL + MySQL billing connection | readiness probe     |
| `/health`         | everything                           | manual check         |

---

## ✨ Features

### Completed

- 📡 **SNMP router polling** — ports, ARP, MAC tables, VLANs (Juniper / Huawei / Extreme / Cisco / FortiGate)
- 👥 **Billing clients** — local database synchronization with the remote billing system and fast search
- 🔎 **Compound VLAN search** — clients and network devices in a single query
- 🖥️ **Dashboards** — filters, sorting, server-side pagination
- 🚪 **Gate management** — create, update, delete with duplicate checks
- 🛡️ **Admin panel** — billing data refresh and cleanup
- 🔐 **Role-based access** — Admin / User with protected routes and API
- 🧵 **Correlation ID** — per-request ID threaded through Serilog's log context (controller → MediatR → repositories) and echoed back on the response
- 🏥 **Health checks** — `/health/live`, `/health/ready`, `/health` (see above)
- 🔢 **API versioning** — all routes under `/api/v1`
- 🐳 **Docker Compose** — API + PostgreSQL, migrations and identity seeding run automatically on startup (see [Quick Start](#-quick-start))

### Planned

- 🖥️ Web frontend (previously part of this repo under `client/`, removed for now — see [Frontend](#frontend))
- 🧪 CI/CD
- 🧩 Refactoring the SNMP import into vendor-specific strategies

---

## ⚙️ Configuration

| Variable                                | Description                                    |
| --------------------------------------- | ---------------------------------------------- |
| `ConnectionStrings__DefaultConnection`  | PostgreSQL connection string                   |
| `ConnectionStrings__DefaultConnection2` | remote MySQL billing database                  |
| `JwtSettings__Key`                      | JWT signing key (min. 32 characters, required) |
| `IdentitySeed__AdminEmail`              | default administrator email                    |
| `IdentitySeed__AdminPassword`           | administrator password (empty — do not create) |

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

## 🧪 Tests

```bash
dotnet test Tests/Tests.csproj
```

514 unit tests across Domain, Application, Infrastructure, Persistence, and API — mocks, EF Core InMemory, and SQLite standing in for MySQL. No external services required.

### Integration tests

```bash
dotnet test Tests.Integration/Tests.Integration.csproj
```

20 tests that boot the real API pipeline (the same startup sequence as `Program.cs` — migrations, identity seeding, the full middleware pipeline) against **ephemeral PostgreSQL and MySQL containers** started via [Testcontainers](https://testcontainers.com/) — entirely separate from any database already running on the machine, torn down after the run. Requires **Docker** to be running; otherwise these fail to start the containers. Kept in a separate project (and out of plain `dotnet test` at the repo root) so the fast unit suite stays Docker-free.

Covers what the unit suite structurally cannot: real EF Core migrations actually applying to Postgres, `ILIKE` filtering executing against a real Npgsql provider, the JWT/cookie/role-authorization pipeline end-to-end over real HTTP, health checks against live dependencies, and `SET SESSION TRANSACTION READ ONLY` genuinely rejecting a write on the MySQL connection.

---

## 📡 SNMP Simulator (manual testing without real hardware)

[`SnmpSimulator/`](SnmpSimulator/) is a standalone SNMP v2c server that replays a captured `snmpwalk` dump — a stand-in router for manually exercising `POST /api/v1/NetworkDevices`, `SNMPExecutor/Get`, `SNMPExecutor/Walk`, etc. without needing a real device on hand. Full details, including how to capture a dump from a real router, are in [`SnmpSimulator/README.md`](SnmpSimulator/README.md).

Two ways to run it:

- **Quick local check** — `dotnet run` it directly on `127.0.0.1:1161` (its defaults) and point ad-hoc `snmpget`/`snmpwalk` calls at that address. Fastest way to validate a dump file, but the API only ever talks SNMP on port 161 to a device's own IP (see `SNMPCommandExecutor.cs`), so this mode doesn't exercise the API itself.
- **Through the API, at a real router's IP** — `docker-compose.snmp.yml` runs the simulator in Docker, pinned to a static IP (e.g. `192.168.101.8`) on the same `192.168.101.0/24` network the `api` container joins in `docker-compose.yml`. The API then talks to it exactly as it would a real router — same port, same IP shape, same code path — which is the closest thing to testing production behavior without real hardware:

  ```bash
  docker compose up -d                                              # once, so the shared network exists
  docker compose -f docker-compose.snmp.yml up -d --build router-8  # bring up the router(s) you need
  # point the app at 192.168.101.8 (SNMP port 161, community "public") as you would a real device
  docker compose -f docker-compose.snmp.yml down                    # tear the simulators down when done
  ```

  This only works because the machine running Docker isn't simultaneously on a real `192.168.101.0/24` network — see the comment above the `app-net` network in `docker-compose.yml`, and [`SnmpSimulator/README.md`](SnmpSimulator/README.md) for adding more simulated devices.

---

## 📄 License

See [LICENSE.md](LICENSE.md).
