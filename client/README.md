# Remote Shell Interrupt — Web Client

React 19 + TypeScript + Vite SPA for the Remote Shell Interrupt backend
(ASP.NET Core API in `../src`). Built with MUI, TanStack Query,
React Hook Form + Zod, Axios and React Router.

## Getting started

```bash
npm install
npm run dev      # dev server on http://localhost:3000
npm run build    # typecheck + production build
npm run lint     # ESLint
npm run preview  # serve the production build
```

The API base URL is read from `VITE_API_URL`. Copy `.env.example` to `.env`
(or create `.env.development` / `.env.production`) and point it at the backend
port — see `../src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API/Properties/launchSettings.json`.

## Architecture

```
src/
├── main.tsx              # Entry point: providers (QueryClient, router, toasts)
├── app/
│   ├── layout/           # App shell: NavBar, global busy indicator, styles
│   ├── router/           # Route table; feature pages are lazy-loaded chunks
│   └── shared/           # Cross-feature presentational components
├── features/             # One folder per domain area (Clients, Gates, ...)
│   └── <Area>/
│       ├── List/         # Dashboard + cards + filters (page-level state)
│       └── Detail/       # Detail views
└── lib/
    ├── api/              # Axios agent, request-param builder, paged fetch
    ├── hooks/            # TanStack Query hooks per domain (useGates, ...)
    ├── schemas/          # Zod validation schemas (react-hook-form resolvers)
    ├── stores/           # Global UI state (useSyncExternalStore based)
    ├── types/            # API DTO types, mirroring the backend camelCase JSON
    └── utils.ts          # Formatting helpers
```

Conventions:

- **Data flow** — components call hooks from `lib/hooks`; hooks are the only
  place that talks to `lib/api`. Query keys are prefixed by domain
  (`"gates"`, `"clients"`, ...) so mutations invalidate whole families.
- **Pagination** — list endpoints return the page body plus an `X-Pagination`
  header (PascalCase JSON, produced by a plain `JsonSerializer.Serialize` on
  the backend). `fetchPaged` merges both into a `PagedResponse<T>`.
- **Error payloads** — the backend middleware also serializes errors with
  plain `JsonSerializer.Serialize`, so `ApiErrorResponse` is PascalCase while
  DTO bodies are camelCase. Don't "normalize" one without the other.
- **DTO casing** — types in `lib/types` mirror the serialized JSON exactly;
  MUI components and forms use camelCase.
