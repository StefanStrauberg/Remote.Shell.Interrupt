import { useEffect } from "react";
import { ErrorOutline, Home, Refresh } from "@mui/icons-material";
import { Button } from "@mui/material";
import { Link, isRouteErrorResponse, useRouteError } from "react-router";
import { reportError } from "@/lib/observability/reportError";
import { routes } from "./paths";
import StatusPage from "../shared/components/StatusPage";

export default function RouteErrorPage() {
  const error = useRouteError();
  const status = isRouteErrorResponse(error) ? error.status : undefined;

  useEffect(() => {
    reportError(error, { boundary: "router", status });
  }, [error, status]);

  return (
    <StatusPage
      fullScreen
      tone="error"
      icon={<ErrorOutline />}
      title={status ? `Request failed (${status})` : "Something went wrong"}
      description="The page could not be displayed. You can retry or return to the home page."
      actions={
        <>
          <Button
            variant="contained"
            startIcon={<Refresh />}
            onClick={() => window.location.reload()}
          >
            Retry
          </Button>
          <Button
            variant="outlined"
            startIcon={<Home />}
            component={Link}
            to={routes.home}
          >
            Home
          </Button>
        </>
      }
    />
  );
}
