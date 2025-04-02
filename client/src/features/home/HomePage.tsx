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
        gap: 4,
        alignItems: "center",
        alignContent: "center",
        justifyContent: "center",
        height: "100vh",
        backgroundImage: "linear-gradient(135deg, #1d3557, #457b9d, #1d3557)",
        boxShadow: "0 8px 20px rgba(0, 0, 0, 0.2)", // Add depth with shadow
      }}
    >
      {/* Logo Section */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 3,
          animation: "fadeIn 3s ease", // Smooth fade-in animation
          "@keyframes fadeIn": {
            from: { opacity: 0 },
            to: { opacity: 1 },
          },
        }}
      >
        <Storage
          sx={{
            height: 120,
            width: 120,
            color: "#e5e7eb", // Highlighted icon color
          }}
        />
        <Typography
          variant="h2"
          fontWeight="bold"
          sx={{
            fontFamily: "'Poppins', sans-serif",
            textShadow: "2px 2px 6px rgba(0, 0, 0, 0.3)", // Subtle text shadow
          }}
        >
          Remote Shell Interrupt
        </Typography>
      </Box>

      {/* Action Button */}
      <Button
        component={Link}
        to="/networkDevices"
        size="large"
        variant="contained"
        sx={{
          backgroundColor: "#e5e7eb",
          color: "#1d3557",
          height: 60,
          width: "fit-content",
          padding: "0 2rem",
          borderRadius: "8px",
          fontSize: "1.5rem",
          fontWeight: "bold",
          textTransform: "none",
          boxShadow: "0 4px 12px rgba(0, 0, 0, 0.2)", // Add shadow to button
          transition: "all 0.3s ease",
          "&:hover": {
            backgroundColor: "#ffd166", // Slightly lighter hover effect
            transform: "scale(1.05)", // Subtle scaling on hover
          },
        }}
      >
        Check up Network Devices
      </Button>
    </Paper>
  );
}
