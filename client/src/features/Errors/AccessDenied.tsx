import { GppBad } from "@mui/icons-material";
import { Box, Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router";
import { useLocation } from "react-router";

export default function AccessDenied() {
  const location = useLocation();
  const attemptedPath = (location.state as { from?: string } | null)?.from;

  return (
    <Paper
      sx={{
        height: "80vh",
        mt: 3,
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        gap: 2
      }}
    >
      <GppBad sx={{ fontSize: 100 }} color="error" />
      <Typography gutterBottom variant="h3" component="h1">
        403 — Access denied
      </Typography>
      <Typography variant="body1" color="text.secondary" textAlign="center" sx={{ maxWidth: 560 }}>
        Your account does not have permission to view this page
        {attemptedPath ? ` (${attemptedPath})` : ""}. Contact an administrator if you
        believe this is a mistake.
      </Typography>
      <Box display="flex" gap={2} mt={2}>
        <Button variant="contained" component={Link} to="/mainPage">
          Back to dashboard
        </Button>
        <Button variant="outlined" component={Link} to="/">
          Home
        </Button>
      </Box>
    </Paper>
  );
}
