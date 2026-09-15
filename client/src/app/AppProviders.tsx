import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "react-router";
import { ToastContainer } from "react-toastify";
import ApplicationErrorBoundary from "./ApplicationErrorBoundary";
import { router } from "./router/Routes";
import { queryClient } from "@/lib/queryClient";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { appTheme } from "./theme";

export default function AppProviders() {
  return (
    <ApplicationErrorBoundary>
      <ThemeProvider theme={appTheme}>
        <CssBaseline />
        <QueryClientProvider client={queryClient}>
          <ToastContainer position="bottom-right" theme="colored" />
          <RouterProvider router={router} />
        </QueryClientProvider>
      </ThemeProvider>
    </ApplicationErrorBoundary>
  );
}
