import { ErrorOutline } from "@mui/icons-material";
import { Alert, Stack, Typography } from "@mui/material";
import { useLocation } from "react-router";
import { ApiErrorResponse } from "../../lib/types/Common/ApiErrorResponse";
import StatusPage from "../../app/shared/components/StatusPage";

type ServerErrorState = {
  error: ApiErrorResponse;
};

export default function ServerError() {
  const { state } = useLocation();
  const error = (state as ServerErrorState | null)?.error;

  const validationErrors = error?.Errors
    ? Object.entries(error.Errors)
        .map(([field, messages]) => `${field}: ${messages.join(", ")}`)
        .join("; ")
    : undefined;

  return (
    <StatusPage
      icon={<ErrorOutline />}
      tone="error"
      title={error?.Title || "Server error"}
      description={
        error?.Detail ||
        "The server could not complete the request. Try again later."
      }
      details={
        error && (
          <Alert severity="error" variant="outlined">
            <Stack gap={0.5} textAlign="left">
              {error.Status && (
                <Typography variant="body2">Status: {error.Status}</Typography>
              )}
              {validationErrors && (
                <Typography variant="body2">{validationErrors}</Typography>
              )}
            </Stack>
          </Alert>
        )
      }
    />
  );
}
