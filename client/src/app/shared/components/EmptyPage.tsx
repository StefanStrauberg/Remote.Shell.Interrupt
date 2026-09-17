import { SearchOff } from "@mui/icons-material";
import { alpha, Box, Paper, Typography, useTheme } from "@mui/material";

type Props = {
  input: string;
  description?: string;
};

export default function EmptyPage({ input, description }: Props) {
  const theme = useTheme();
  return (
    <Paper
      variant="outlined"
      sx={{
        minHeight: 360,
        display: "grid",
        placeItems: "center",
        p: 4,
        backgroundImage: `radial-gradient(ellipse at 50% 40%, ${alpha(
          theme.palette.primary.main,
          theme.palette.mode === "dark" ? 0.12 : 0.06
        )} 0%, transparent 65%)`,
      }}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          gap: 1.5,
          maxWidth: 520,
          textAlign: "center",
        }}
      >
        <Box
          sx={{
            width: 64,
            height: 64,
            display: "grid",
            placeItems: "center",
            color: "primary.main",
            bgcolor: "primary.light",
            borderRadius: "50%",
            mb: 0.5,
          }}
        >
          <SearchOff fontSize="large" />
        </Box>
        <Typography variant="h5" component="h2">
          {input}
        </Typography>
        {description && (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ maxWidth: 420 }}
          >
            {description}
          </Typography>
        )}
      </Box>
    </Paper>
  );
}
