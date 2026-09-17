# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A network infrastructure monitoring platform (ASP.NET Core 9, Clean Architecture, CQRS) that polls routers over SNMP and syncs with a read-only billing database, plus a React 19 + TypeScript SPA. See [README.md](README.md) for the full architecture diagram, feature list, auth model, and Docker Compose setup, and [client/README.md](client/README.md) for the frontend's directory layout and conventions — don't duplicate those here.

## Commands

### Backend

```bash
dotnet run --project src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API   # applies EF migrations + seeds identity on startup
dotnet test Tests/Tests.csproj                                                                      # unit tests (mocks/InMemory/SQLite, no Docker)
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~CreateGateCommandHandler"                # single test/class
dotnet test Tests.Integration/Tests.Integration.csproj                                              # real Postgres+MySQL via Testcontainers, needs Docker
```

Adding a migration (after changing an entity or `IEntityTypeConfiguration<T>`):

```bash
dotnet tool restore   # once, installs the pinned dotnet-ef from .config/dotnet-tools.json
dotnet ef migrations add <DescriptiveName> \
  --project src/Remote.Shell.Interrupt.Storehouse/Persistence/Remote.Shell.Interrupt.Storehouse.Dapper.Persistence \
  --startup-project src/Remote.Shell.Interrupt.Storehouse/Persistence/Remote.Shell.Interrupt.Storehouse.Dapper.Persistence \
  --output-dir Migrations
```

(No running database needed — the Persistence project is its own design-time factory via `ApplicationDbContextFactory`.)

### Frontend (run from `client/`)

```bash
npm run dev              # http://localhost:3000
npm run check             # format:check + lint + typecheck + test + build — the single CI-equivalent gate
npm test                  # vitest run
npx vitest run tests/gates-ui.test.tsx   # single test file
npm run format             # apply Prettier fixes
npm run test:coverage      # HTML report at client/coverage/index.html
```

## Architecture

### Backend layering (Clean Architecture)

```
Core/Domain            — entities, no dependencies on other layers
Core/Application        — CQRS commands/queries+handlers, DTOs, validation, contracts (interfaces) for everything Infrastructure/Persistence implement
Infrastructure/*        — SNMPCommandExecutor, WorkflowEngine, Specification, QueryFilterParser, AppLogger: each a separate project implementing Application's contracts
Persistence/*.Dapper.Persistence — EF Core (PostgreSQL, owned schema, migrations), ASP.NET Identity, Dapper (MySQL billing, read-only)
Remote.Shell.Interrupt.Storehouse.API — ASP.NET Core host: controllers, DI wiring, middleware
```

Dependency direction is inward: Domain has no references; Application references only Domain and defines interfaces (`Contracts/`); every Infrastructure/Persistence project implements those interfaces and is wired up in the API host's DI. When adding a capability, define the contract in `Application/Contracts`, implement it in the appropriate Infrastructure or Persistence project, then register it in the API's DI setup.

**Why two data-access technologies** (EF Core vs Dapper): PostgreSQL is owned by this app (EF Core migrations, full schema control). The MySQL billing database is third-party — only specific columns/tables are readable, and it must never be written to. Dapper's "run this SQL, map these columns" model fits that; the connection also issues `SET SESSION TRANSACTION READ ONLY` on open so a coding mistake can't write to it even bypassing code review.

### CQRS feature structure

Each `Application/Features/<Area>/` groups `Commands/<Verb><Entity>/` and `Queries/<Verb><Entity>/`, each folder holding the `I(Command|Query)` record plus its `internal` `I(Command|Query)Handler` (see [CreateGateCommandHandler.cs](src/Remote.Shell.Interrupt.Storehouse/Core/Remote.Shell.Interrupt.Storehouse.Application/Features/Gates/Commands/CreateGate/CreateGateCommandHandler.cs) for the pattern). MediatR dispatches; `Behaviors/ValidationBehavior` and `Behaviors/LoggingBehavior` wrap every request. Many create/update/delete handlers derive from generic base handlers (`CreateEntityCommandHandler<TEntity, TDto, TCommand>`, etc.) that take an `ISpecification<T>` for duplicate-checking and an `IMapper` (Mapster's `MapsterMapper.IMapper`) for DTO mapping — override the few hook methods (`BuildDuplicateCheckFilter`, `ValidateEntityDoesNotExistAsync`, `PersistNewEntity`) rather than reimplementing CRUD. Each DTO implements Mapster's `IRegister.Register(TypeAdapterConfig)` to declare its own mapping (see [GateDTO.cs](src/Remote.Shell.Interrupt.Storehouse/Core/Remote.Shell.Interrupt.Storehouse.Application/DTOs/Gates/GateDTO.cs)); `ApplicationServicesRegistration` scans the assembly for these at startup instead of a per-DTO manual registration list.

Filtering/sorting/pagination for list queries goes through `Infrastructure/*.QueryFilterParser` (parses `RequestParameters.Filters` into LINQ expressions) combined with `Infrastructure/*.Specification` (`ISpecification<T>` criteria objects) — see `IGateSpecification`/`GateSpecificationTests` for the shape.

### Workflow engine

Graphs of typed nodes (`Start`/`End`/`Decision`/`Join`/`SetVariable`/`SnmpGet`/`SnmpWalk`/`Script`/`SaveNetworkDevice`) live in `Infrastructure/*.WorkflowEngine`. `WorkflowEngine.cs` walks the graph via `WorkflowNodeResolver`, dispatching to one `NodeExecutors/<Type>NodeExecutor` per node type. `Script` nodes run sandboxed JavaScript through Jint (`function execute(input, context)` contract, `console.log` captured per step — see `WorkflowScriptPrelude.cs`). Graphs have a `Draft → Published → Archived` lifecycle; a Published graph is immutable. The seeded "Network device discovery" workflow (vendor-specific Juniper/Huawei/Extreme/Cisco/FortiGate SNMP parsing) is what `POST /NetworkDevices/CreateNetworkDevice` actually runs — this replaced a large hand-coded handler, so new vendor-specific logic belongs in workflow graphs/scripts, not new C# branches in a controller/handler.

### Frontend

See [client/README.md](client/README.md) — feature-sliced (`src/features/<Area>/{api,List,Detail}`), strict dependency direction (UI → feature query hooks → feature API → shared HTTP transport), typed query-key factories per domain, one HTTP transport owning cookies/401 handling. Note the **casing split**: DTO bodies are camelCase (`System.Text.Json` defaults) but `X-Pagination` headers and `ApiErrorResponse` error payloads are PascalCase (backend uses plain `JsonSerializer.Serialize` for those) — don't "fix" one without checking the other.

### Testing layout

- `Tests/` — unit tests (xUnit, NSubstitute, FluentAssertions), mirrors the `src/` layer structure (`Domain/`, `Application/` incl. `Features`/`Validations`/`Behaviors`, `Infrastructure/`, `Persistence/`, `Api/`). No external services; MySQL is stood in with SQLite. Uses one `GlobalUsing.cs` for shared usings — add new cross-cutting usings there rather than per-file.
- `Tests.Integration/` — boots the real API pipeline against ephemeral Postgres+MySQL Testcontainers (needs Docker running); kept as a separate project/csproj so plain `dotnet test` at the repo root (if ever run without a target) and CI's fast path stay Docker-free.
- `client/tests/` — Vitest + React Testing Library, mocks the feature API boundary, real forms/validation/routing; `tests/renderApp.tsx` gives each test an isolated query cache/theme/router.

## Conventions worth knowing

- Indentation is 2 spaces in C# (see any handler file) — matches the frontend's Prettier config.
- New Infrastructure/Persistence implementations are `internal` where possible (e.g. command handlers), exposed only through their Application-layer interface.
- API routes are versioned under `/api/v1`; the SPA talks to them through the cookie-auth flow (`Auth/CookieLogin`), not the JWT bearer flow (that's kept for non-browser API clients).
