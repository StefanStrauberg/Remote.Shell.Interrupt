import { Alert, Box, CircularProgress, Paper, Typography } from "@mui/material";

export function PageLoading({ message = "Loading…" }: { message?: string }) {
  return (
    <Paper
      variant="outlined"
      sx={{
        minHeight: 320,
        display: "grid",
        placeItems: "center",
        p: 4,
      }}
    >
      <Box textAlign="center">
        <CircularProgress size={42} thickness={4} />
        <Typography color="text.secondary" mt={2}>
          {message}
        </Typography>
      </Box>
    </Paper>
  );
}

export function PageError({
  title = "Unable to load data",
  error,
}: {
  title?: string;
  error: unknown;
}) {
  const message =
    error instanceof Error ? error.message : "An unknown error occurred.";

  return (
    <Alert severity="error" variant="outlined">
      <Box>
        <Typography fontWeight={700}>{title}</Typography>
        <Typography variant="body2">{message}</Typography>
      </Box>
    </Alert>
  );
}
