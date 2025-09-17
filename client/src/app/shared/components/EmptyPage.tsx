import { Box, Paper, Typography } from "@mui/material";

type Props = {
  input: string;
  description?: string; // FIXED: Make description optional
};

export default function EmptyPage({ input, description }: Props) {
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
        boxShadow: "0 8px 20px rgba(0, 0, 0, 0.2)",
      }}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          gap: 3,
          animation: "fadeIn 3s ease",
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
            textShadow: "2px 2px 6px rgba(0, 0, 0, 0.3)",
          }}
        >
          {input}
        </Typography>
        {/* FIXED: Only show description if provided */}
        {description && (
          <Typography
            variant="body1"
            sx={{
              fontFamily: "'Poppins', sans-serif",
              textShadow: "1px 1px 3px rgba(0, 0, 0, 0.3)",
            }}
          >
            {description}
          </Typography>
        )}
      </Box>
    </Paper>
  );
}
