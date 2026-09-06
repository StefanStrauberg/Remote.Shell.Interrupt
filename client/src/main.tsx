import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router";
import { QueryClientProvider } from "@tanstack/react-query";
import { router } from "./app/router/Routes.tsx";
import { queryClient } from "./lib/queryClient.ts";
import { ToastContainer } from "react-toastify";
import { useAuthStore } from "./lib/auth/authStore.ts";

// Silent session check: validates the persisted token and restores the
// profile before the first render, so page refreshes keep the session.
useAuthStore.getState().restoreSession();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <ToastContainer position="bottom-right" theme="dark" />
      <RouterProvider router={router} />
    </QueryClientProvider>
  </StrictMode>
);
