import { Storage } from "@mui/icons-material";
import { Box, Paper, Typography } from "@mui/material";
import { ReactNode } from "react";
import { designTokens } from "@/app/theme";

type Props = {
  title: string;
  subtitle: string;
  children: ReactNode;
  maxWidth?: number;
};

export default function AuthLayout({
  title,
  subtitle,
  children,
  maxWidth = 420,
}: Props) {
  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "grid",
        placeItems: "center",
        px: 2,
        py: 4,
        backgroundImage: designTokens.gradients.brand,
        position: "relative",
        overflow: "hidden",
        "&::before": {
          content: '""',
          position: "absolute",
          inset: 0,
          background:
            "radial-gradient(circle at 20% 15%, rgba(255,255,255,0.14), transparent 35%)",
        },
      }}
    >
      <Paper
        component="main"
        sx={{
          width: "100%",
          maxWidth,
          p: { xs: 3, sm: 4 },
          borderRadius: 3,
          boxShadow: designTokens.shadows.floating,
          position: "relative",
        }}
      >
        <Box textAlign="center" mb={3}>
          <Box
            sx={{
              width: 56,
              height: 56,
              display: "grid",
              placeItems: "center",
              mx: "auto",
              mb: 1.5,
              color: "primary.main",
              bgcolor: "primary.light",
              borderRadius: 2.5,
            }}
          >
            <Storage fontSize="large" />
          </Box>
          <Typography variant="h4" component="h1">
            {title}
          </Typography>
          <Typography variant="body2" color="text.secondary" mt={0.75}>
            {subtitle}
          </Typography>
        </Box>
        {children}
      </Paper>
    </Box>
  );
}
