import { FilterList } from "@mui/icons-material";
import {
  Box,
  ListItemText,
  MenuItem,
  MenuList,
  Paper,
  Typography,
} from "@mui/material";

export default function TfPlanFilter() {
  return (
    <Box
      sx={{ display: "flex", flexDirection: "column", gap: 3, borderRadius: 3 }}
    >
      <Paper sx={{ p: 3, borderRadius: 3 }}>
        <Box sx={{ width: "100%" }}>
          <Typography
            variant="h6"
            sx={{
              display: "flex",
              alignItems: "center",
              mb: 1,
              color: "primary.main",
            }}
          >
            <FilterList sx={{ mr: 1 }} />
            Filters
          </Typography>
          <MenuList>
            <MenuItem>
              <ListItemText primary="Name: " />
            </MenuItem>
            <MenuItem>
              <ListItemText primary="Working:" />
            </MenuItem>
            <MenuItem>
              <ListItemText primary="AntiDDOS:" />
            </MenuItem>
          </MenuList>
        </Box>
      </Paper>
    </Box>
  );
}
