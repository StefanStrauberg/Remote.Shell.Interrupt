import { Box, Stack, Typography } from "@mui/material";
import { SvgIconComponent } from "@mui/icons-material";
import { ReactNode } from "react";

type Props = {
  title: string;
  description?: string;
  icon?: SvgIconComponent;
  action?: ReactNode;
};

export default function PageHeader({
  title,
  description,
  icon: Icon,
  action,
}: Props) {
  return (
    <Stack
      direction={{ xs: "column", sm: "row" }}
      alignItems={{ xs: "flex-start", sm: "center" }}
      justifyContent="space-between"
      gap={2}
      mb={3}
    >
      <Stack direction="row" alignItems="center" gap={1.5}>
        {Icon && (
          <Box
            aria-hidden
            sx={{
              width: 44,
              height: 44,
              display: "grid",
              placeItems: "center",
              color: "primary.main",
              bgcolor: "primary.light",
              borderRadius: 2.5,
            }}
          >
            <Icon />
          </Box>
        )}
        <Box>
          <Typography variant="h4" component="h1">
            {title}
          </Typography>
          {description && (
            <Typography variant="body2" color="text.secondary" mt={0.25}>
              {description}
            </Typography>
          )}
        </Box>
      </Stack>
      {action}
    </Stack>
  );
}
