import { Box, Paper, Typography } from "@mui/material";

export default function ClientsEmptyPage() {
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
        height: "90vh",
        backgroundImage: "linear-gradient(135deg, #1d3557, #457b9d, #1d3557)",
        boxShadow: "0 8px 20px rgba(0, 0, 0, 0.2)", // Add depth with shadow
      }}
    >
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
        <Typography
          variant="h3"
          fontWeight="bold"
          sx={{
            fontFamily: "'Poppins', sans-serif",
            textShadow: "2px 2px 6px rgba(0, 0, 0, 0.3)", // Subtle text shadow
          }}
        >
          Клиенты не найдены
        </Typography>
      </Box>
    </Paper>
  );
}
