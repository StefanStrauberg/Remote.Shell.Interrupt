import { Box, Paper, Stack, Typography } from "@mui/material";
import { ReactNode } from "react";

type Props = {
  icon: ReactNode;
  title: string;
  description: ReactNode;
  actions?: ReactNode;
  details?: ReactNode;
  fullScreen?: boolean;
  tone?: "primary" | "error" | "warning";
};

export default function StatusPage({
  icon,
  title,
  description,
  actions,
  details,
  fullScreen = false,
  tone = "primary",
}: Props) {
  return (
    <Box
      sx={{
        minHeight: fullScreen ? "100vh" : "60vh",
        display: "grid",
        placeItems: "center",
        p: { xs: 2, sm: 3 },
      }}
    >
      <Paper
        variant="outlined"
        sx={{ width: "100%", maxWidth: 680, p: { xs: 3, sm: 5 } }}
      >
        <Stack alignItems="center" textAlign="center" gap={2}>
          <Box
            sx={{
              width: 72,
              height: 72,
              display: "grid",
              placeItems: "center",
              borderRadius: "50%",
              color: `${tone}.main`,
              bgcolor: `${tone}.light`,
              "& svg": { fontSize: 38 },
            }}
          >
            {icon}
          </Box>
          <Typography variant="h3" component="h1">
            {title}
          </Typography>
          <Typography color="text.secondary" sx={{ maxWidth: 520 }}>
            {description}
          </Typography>
          {details && <Box width="100%">{details}</Box>}
          {actions && (
            <Stack
              direction={{ xs: "column", sm: "row" }}
              justifyContent="center"
              gap={1.5}
              mt={1}
            >
              {actions}
            </Stack>
          )}
        </Stack>
      </Paper>
    </Box>
  );
}
