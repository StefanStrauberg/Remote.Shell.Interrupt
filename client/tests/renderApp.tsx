import { ReactElement } from "react";
import { render } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { MemoryRouter, useLocation } from "react-router";
import { ThemeProvider } from "@mui/material";
import { appTheme } from "../src/app/theme";
import { afterEach } from "vitest";

const clients = new Set<QueryClient>();
afterEach(() => {
  clients.forEach((client) => client.clear());
  clients.clear();
});

function LocationProbe() {
  const location = useLocation();
  return (
    <output aria-label="Current location">
      {location.pathname}
      {location.search}
      {location.hash}|{JSON.stringify(location.state)}
    </output>
  );
}

export function renderApp(
  ui: ReactElement,
  entry: string | { pathname: string; state: unknown } = "/"
) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: Infinity },
      mutations: { retry: false },
    },
  });
  clients.add(queryClient);
  const result = render(
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={appTheme}>
        <MemoryRouter initialEntries={[entry]}>
          {ui}
          <LocationProbe />
        </MemoryRouter>
      </ThemeProvider>
    </QueryClientProvider>
  );
  return { ...result, queryClient };
}

export function deferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (reason: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}
