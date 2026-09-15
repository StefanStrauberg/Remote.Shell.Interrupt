import { Box, Paper, Typography, Chip } from "@mui/material";
import { Fingerprint } from "@mui/icons-material";

type Props = {
  macTable: string[];
};

export default function NetworkDeviceMACTable({ macTable }: Props) {
  return (
    <Paper elevation={1} sx={{ p: 2, backgroundColor: "grey.50" }}>
      <Typography variant="subtitle2" fontWeight="bold" gutterBottom>
        MAC Addresses:
      </Typography>
      <Box display="flex" flexWrap="wrap" gap={1}>
        {macTable.map((mac, index) => (
          <Chip
            key={index}
            icon={<Fingerprint />}
            label={mac}
            size="small"
            variant="outlined"
            sx={{ fontFamily: "monospace" }}
          />
        ))}
      </Box>
    </Paper>
  );
}
