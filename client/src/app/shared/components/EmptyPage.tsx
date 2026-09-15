import { SearchOff } from "@mui/icons-material";
import { Box, Paper, Typography } from "@mui/material";

type Props = {
  input: string;
  description?: string;
};

export default function EmptyPage({ input, description }: Props) {
  return (
    <Paper
      variant="outlined"
      sx={{ minHeight: 320, display: "grid", placeItems: "center", p: 4 }}
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
