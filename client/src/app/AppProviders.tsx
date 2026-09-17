import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "react-router";
import { ToastContainer } from "react-toastify";
import ApplicationErrorBoundary from "./ApplicationErrorBoundary";
import { router } from "./router/Routes";
import { queryClient } from "@/lib/queryClient";
import ThemeModeProvider from "./ThemeModeContext";

export default function AppProviders() {
  return (
    <ApplicationErrorBoundary>
      <ThemeModeProvider>
        <QueryClientProvider client={queryClient}>
          <ToastContainer position="bottom-right" theme="colored" />
          <RouterProvider router={router} />
        </QueryClientProvider>
      </ThemeModeProvider>
    </ApplicationErrorBoundary>
  );
}
