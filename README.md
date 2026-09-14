# 🌐 Remote.Shell.Interrupt

**A comprehensive network infrastructure monitoring platform:** SNMP-based data collection from routers, billing system synchronization, and a role-based React web interface.

- **Backend** — .NET 9, Clean Architecture, CQRS
- **Frontend** — React 19 + TypeScript + Vite
- **Databases** — PostgreSQL (primary) + MySQL (billing gateway)
- **Tests** — xUnit, 304 tests

---

## 🧱 Architecture

```text
Remote.Shell.Interrupt/
├── Remote.Shell.Interrupt.sln
├── src/Remote.Shell.Interrupt.Storehouse/
│   ├── Core/
│   │   ├── ...Storehouse.Domain        # Domain entities
│   │   └── ...Storehouse.Application   # CQRS, DTOs, validation, contracts
│   ├── Infrastructure/                 # SNMP, logger, specifications, filter parser
│   ├── Persistence/                    # EF Core (PostgreSQL), Identity, Dapper (MySQL)
│   └── Remote.Shell.Interrupt.Storehouse.API/  # ASP.NET Core 9 — API host
├── client/                             # React 19 + Vite — web interface (see client/README.md)
└── Tests/                              # xUnit — 304 tests
```

---

## 🛠️ Technologies

### Backend

- **ASP.NET Core 9** — REST API
- **EF Core 9 + Npgsql** — PostgreSQL data access
- **ASP.NET Core Identity** — authentication (JWT + Cookie), roles
- **MediatR** — CQRS pipeline (validation + logging)
- **AutoMapper** — DTO mapping
- **FluentValidation** — command validation
- **Serilog** — structured logging (console + files)
- **SharpSnmpLib** — SNMP v2c
- **Dapper + MySql.Data** — read-only gateway to the remote billing database

> **Why two data-access technologies?** PostgreSQL is owned by this application (full schema knowledge, EF Core migrations). The MySQL billing database is owned by a third party: its full schema is unknown, this app is only permitted to read specific columns from specific tables, and it must never write to it. EF Core wants to fully model and evolve a schema it owns, which doesn't fit that constraint — Dapper's "run this SQL, map these columns" model does. The MySQL connection additionally issues `SET SESSION TRANSACTION READ ONLY` on open, so even a future coding mistake that tried to write would be rejected by the database itself, not just by code review.

### Frontend

- **React 19 + TypeScript** — SPA
- **Vite 6** — build tooling and dev server
- **MUI 6** — UI components
- **TanStack Query 5** — server state
- **Zustand 5** — global auth state
- **React Hook Form + Zod** — validated forms
- **Axios** — HTTP client with interceptors

---

## 🚀 Quick Start

### Requirements

- **.NET 9 SDK**
- **Node.js 20+** and npm
- **PostgreSQL 14+**
- MySQL — only for billing synchronization

### 1. Backend

```bash
dotnet run --project src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API
```

On startup the API **automatically** applies pending EF Core migrations (creating the full schema from scratch on a fresh/empty database, e.g. a newly deployed container) and creates the roles and the administrator account.

### 2. Frontend

```bash
cd client
npm install
npm run dev
```

### 3. Sign in (Development configuration)

| Parameter | Value                   |
| --------- | ----------------------- |
| URL       | http://localhost:3000   |
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
| Gates: create / update / delete                |  ✅   |  ❌  |
| Billing sync and cleanup                       |  ✅   |  ❌  |
| Deleting network devices                       |  ✅   |  ❌  |
| Registering users                              |  ✅   |  ❌  |
| SNMP Get / Walk                                |  ✅   |  ❌  |

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
- 🏥 **Health checks** — `/health/live` (process only), `/health/ready` and `/health` (PostgreSQL + MySQL billing connectivity), for use as liveness/readiness probes

### Planned

- 🐳 Docker Compose for local deployment
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
| `VITE_API_URL`                          | API base URL for the client (`client/.env`)    |

Secrets are provided via user-secrets or environment variables:

```bash
cd src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Key" "signing-key-min-32-characters"
dotnet user-secrets set "IdentitySeed:AdminPassword" "a-strong-password"
```

The client reads `VITE_API_URL` from `client/.env`:

```env
VITE_API_URL=http://localhost:5000
```

---

## 🗄️ Database

PostgreSQL schema is managed with real **EF Core Migrations**, stored in `Persistence/Remote.Shell.Interrupt.Storehouse.Dapper.Persistence/Migrations/`. On startup the API calls `Database.MigrateAsync()`, which:

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
dotnet test
```

---

## 📄 License

See [LICENSE.md](LICENSE.md). Frontend details — [client/README.md](client/README.md).
