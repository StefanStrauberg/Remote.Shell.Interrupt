import { Storage } from "@mui/icons-material";
import { Box, Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router";

export default function HomePage() {
  return (
    <Paper
      sx={{
        color: "white",
        display: "flex",
        flexDirection: "column",
        gap: 6,
        alignItems: "center",
        alignContent: "center",
        justifyContent: "center",
        height: "100vh",
        backgroundImage:
          "linear-gradient(35deg, #182a73 0%, #218aae 69%, #20a7ac)",
      }}
    >
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          alignContent: "center",
          color: "white",
          gap: 3,
        }}
      >
        <Storage sx={{ height: 110, width: 110 }} />
        <Typography variant="h2">Remote Shell Interrupt</Typography>
      </Box>
      <Button
        component={Link}
        to="/NetworkDeviceList"
        size="large"
        variant="contained"
        sx={{ height: 80, borderRadius: 4, fontSize: "1.5rem" }}
      >
        Check up Network Devices
      </Button>
    </Paper>
  );
}
