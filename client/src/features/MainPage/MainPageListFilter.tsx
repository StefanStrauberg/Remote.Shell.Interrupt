import { Box, Button, Stack, TextField, Typography } from "@mui/material";
import { Search, RestartAlt } from "@mui/icons-material";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import { useState } from "react";
import { isValidVlanId } from "../../lib/utils";

type Props = {
  onApplyFilters: (filters: RouterFilter) => void;
  onSearch: () => void;
  initialVlan?: number;
};
export default function MainPageListFilter({
  onApplyFilters,
  onSearch,
  initialVlan,
}: Props) {
  const [idVlan, setIdVlan] = useState<number | null>(initialVlan ?? null);
  const [error, setError] = useState("");
  const apply = () => {
    if (!isValidVlanId(idVlan)) {
      setError("VLAN ID must be a whole number from 1 to 4094");
      return;
    }
    setError("");
    onApplyFilters({ IdVlan: { op: "==", value: idVlan } });
    onSearch();
  };
  return (
    <Box
      component="form"
      onSubmit={(event) => {
        event.preventDefault();
        apply();
      }}
    >
      <Stack direction="row" alignItems="flex-start" gap={1.5} flexWrap="wrap">
        <TextField
          label="VLAN ID"
          type="number"
          size="small"
          value={idVlan ?? ""}
          inputProps={{ min: 1, max: 4094 }}
          placeholder="e.g., 120"
          error={!!error}
          helperText={error || "VLAN range: 1–4094"}
          sx={{ width: { xs: "100%", sm: 240 } }}
          onChange={(e) => {
            const value = e.target.value === "" ? null : Number(e.target.value);
            setIdVlan(value);
            setError(
              value !== null && !isValidVlanId(value)
                ? "VLAN ID must be a whole number from 1 to 4094"
                : ""
            );
          }}
        />
        <Button
          variant="contained"
          type="submit"
          disabled={!isValidVlanId(idVlan)}
          startIcon={<Search />}
        >
          Search
        </Button>
        <Button
          variant="outlined"
          disabled={idVlan === null}
          startIcon={<RestartAlt />}
          onClick={() => {
            setIdVlan(null);
            setError("");
            onApplyFilters({});
          }}
        >
          Reset
        </Button>
        <Typography
          variant="caption"
          color="text.secondary"
          sx={{ ml: { sm: "auto" }, pt: 1 }}
        >
          Search clients and collected device data
        </Typography>
      </Stack>
    </Box>
  );
}
