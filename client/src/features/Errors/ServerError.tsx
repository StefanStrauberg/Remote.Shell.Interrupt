import { Divider, Paper, Typography } from "@mui/material";
import { useLocation } from "react-router";
import { ApiErrorResponse } from "../../lib/types/Common/ApiErrorResponse";

type ServerErrorState = {
  error: ApiErrorResponse;
};

export default function ServerError() {
  const { state } = useLocation();
  const error = (state as ServerErrorState | null)?.error;

  return (
    <Paper>
      {error ? (
        <>
          <Typography
            gutterBottom
            variant="h3"
            sx={{ px: 4, pt: 2 }}
            color="error"
          >
            {error.Title || "There has been an error"}
          </Typography>
          <Divider />
          <Typography variant="body1" sx={{ p: 2 }}>
            Detail: {error.Detail || "Internal server error's detail"}
          </Typography>
          <Typography variant="body1" sx={{ p: 2 }}>
            Status: {error.Status || "Internal server error's status"}
          </Typography>
          <Typography variant="body1" sx={{ p: 2 }}>
            Errors:{" "}
            {error.Errors
              ? Object.entries(error.Errors)
                  .map(([field, messages]) => `${field}: ${messages.join(", ")}`)
                  .join("; ")
              : "Internal server error's errors"}
          </Typography>
        </>
      ) : (
        <Typography variant="h5">Server error</Typography>
      )}
    </Paper>
  );
}
