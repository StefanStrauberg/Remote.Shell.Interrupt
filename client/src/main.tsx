import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { router } from "./app/router/Routes.tsx";
import { routes } from "./app/router/paths.ts";
import { useAuthStore } from "./lib/auth/authStore.ts";
import { registerUnauthorizedHandler } from "./lib/auth/unauthorizedHandler.ts";
import { queryClient } from "./lib/queryClient.ts";
import AppProviders from "./app/AppProviders.tsx";
import "@fontsource/roboto/latin-400.css";
import "@fontsource/roboto/latin-500.css";
import "@fontsource/roboto/latin-700.css";
import "@fontsource/roboto/cyrillic-400.css";
import "@fontsource/roboto/cyrillic-500.css";
import "@fontsource/roboto/cyrillic-700.css";

registerUnauthorizedHandler(async () => {
  useAuthStore.getState().clearSession();
  queryClient.clear();

  if (window.location.pathname !== routes.login) {
    const from = `${window.location.pathname}${window.location.search}${window.location.hash}`;
    await router.navigate(routes.login, { replace: true, state: { from } });
  }
});

// Silent session check: validates the persisted token and restores the
// profile before the first render, so page refreshes keep the session.
useAuthStore.getState().restoreSession();

const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error('Application root element "#root" was not found.');
}

createRoot(rootElement).render(
  <StrictMode>
    <AppProviders />
  </StrictMode>
);
