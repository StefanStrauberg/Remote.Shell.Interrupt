import { ArrowBack, HubOutlined, Terminal } from "@mui/icons-material";
import { Box, Button, Stack, Typography } from "@mui/material";
import { ReactNode } from "react";
import { Link } from "react-router";
import { routes } from "@/app/router/paths";

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
        gridTemplateColumns: { xs: "1fr", md: ".9fr 1.1fr" },
      }}
    >
      <Box
        sx={{
          display: { xs: "none", md: "flex" },
          flexDirection: "column",
          bgcolor: "#14292e",
          color: "white",
          p: 6,
        }}
      >
        <Stack direction="row" gap={1.5} alignItems="center">
          <Terminal sx={{ color: "#73e7c3" }} />
          <Typography fontWeight={650}>Remote Shell / Interrupt</Typography>
        </Stack>
        <Box sx={{ my: "auto", py: 8, maxWidth: 430 }}>
          <HubOutlined sx={{ color: "#73e7c3", fontSize: 64, mb: 4 }} />
          <Typography
            sx={{
              fontSize: { md: 44, lg: 56 },
              fontWeight: 700,
              letterSpacing: "-.045em",
              lineHeight: 1.12,
            }}
          >
            A clearer view.
            <br />A connected
            <br />
            <Box component="span" sx={{ color: "#73e7c3" }}>
              workspace.
            </Box>
          </Typography>
          <Typography sx={{ mt: 3, color: "#a8bdbf", lineHeight: 1.9 }}>
            Everything you need to explore your network infrastructure and keep
            operations moving.
          </Typography>
        </Box>
        <Typography variant="caption" color="#a8bdbf">
          NETWORK INFRASTRUCTURE MANAGEMENT
        </Typography>
      </Box>
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          p: { xs: 3, sm: 5 },
          bgcolor: "background.paper",
        }}
      >
        <Button
          component={Link}
          to={routes.home}
          startIcon={<ArrowBack />}
          color="inherit"
          sx={{ alignSelf: "flex-start" }}
        >
          Back to home
        </Button>
        <Box
          component="main"
          sx={{ width: "100%", maxWidth, mx: "auto", my: "auto", py: 6 }}
        >
          <Typography
            variant="overline"
            color="primary"
            sx={{ letterSpacing: ".16em" }}
          >
            YOUR WORKSPACE AWAITS
          </Typography>
          <Typography variant="h2" component="h1" sx={{ mt: 1, mb: 1 }}>
            {title}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
            {subtitle}
          </Typography>
          {children}
        </Box>
        <Typography variant="caption" color="text.secondary" textAlign="center">
          Remote Shell Interrupt
        </Typography>
      </Box>
    </Box>
  );
}
