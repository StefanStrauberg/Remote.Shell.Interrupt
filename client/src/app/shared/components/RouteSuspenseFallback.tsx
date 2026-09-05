import { Box, CircularProgress } from "@mui/material";

/** Loading placeholder shown while a lazily loaded route chunk downloads. */
export default function RouteSuspenseFallback() {
  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      minHeight="40vh"
    >
      <CircularProgress />
    </Box>
  );
}
