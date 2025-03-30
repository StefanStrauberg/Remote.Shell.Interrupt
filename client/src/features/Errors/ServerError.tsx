import { Divider, Paper, Typography } from "@mui/material";
import { useLocation } from "react-router";

export default function ServerError() {
  const { state } = useLocation();
  return (
    <Paper>
      {state.error ? (
        <>
          <Typography
            gutterBottom
            variant="h3"
            sx={{ px: 4, pt: 2 }}
            color="error"
          >
            {state.error?.message || "There has been an error"}
          </Typography>
          <Divider />
          <Typography variant="body1" sx={{ p: 2 }}>
            Detail: {state.error?.Detail || "Internal server error's detail"}
          </Typography>
          <Typography variant="body1" sx={{ p: 2 }}>
            Errors: {state.error?.Errors || "Internal server error's errors"}
          </Typography>
          <Typography variant="body1" sx={{ p: 2 }}>
            Status: {state.error?.Status || "Internal server error's status"}
          </Typography>
          <Typography variant="body1" sx={{ p: 2 }}>
            Title: {state.error?.Title || "Internal server error's title"}
          </Typography>
        </>
      ) : (
        <Typography variant="h5">Server error</Typography>
      )}
    </Paper>
  );
}
