import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  TextField,
  Chip,
} from "@mui/material";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import { useState } from "react";
import { isValidVlanId } from "../../lib/utils";

type Props = {
  onApplyFilters: (filters: RouterFilter) => void;
  onSearch: () => void;
};

export default function MainPageListFilter({
  onApplyFilters,
  onSearch,
}: Props) {
  const [idVlan, setIdVlan] = useState<number | null>(null);
  const [error, setError] = useState<string>("");

  const handleApplyClick = () => {
    setError("");

    if (!isValidVlanId(idVlan)) {
      setError("VLAN ID must be a whole number from 1 to 4094");
      return;
    }

    const filters: RouterFilter = {
      IdVlan: { op: "==", value: idVlan },
    };

    onApplyFilters(filters);
    onSearch();
  };

  const handleReset = () => {
    setIdVlan(null);
    setError("");
    onApplyFilters({});
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    if (value === "") {
      setIdVlan(null);
      setError("");
    } else {
      const parsedValue = Number(value);
      setIdVlan(Number.isFinite(parsedValue) ? parsedValue : null);
      setError(
        isValidVlanId(parsedValue)
          ? ""
          : "VLAN ID must be a whole number from 1 to 4094"
      );
    }
  };

  return (
    <Card
      sx={{
        borderRadius: 2,
        overflow: "hidden",
        position: "sticky",
        top: 20,
      }}
    >
      <CardHeader
        title={
          <Box display="flex" alignItems="center">
            <FilterList color="primary" sx={{ mr: 1 }} />
            <span>Search Filters</span>
          </Box>
        }
        sx={{
          bgcolor: "grey.50",
          borderBottom: 1,
          borderColor: "divider",
          py: 1.5,
        }}
      />

      <CardContent sx={{ p: 2 }}>
        <Box
          component="form"
          onSubmit={(event) => {
            event.preventDefault();
            handleApplyClick();
          }}
          sx={{ display: "flex", flexDirection: "column", gap: 2 }}
        >
          <TextField
            label="VLAN ID"
            value={idVlan?.toString() || ""}
            onChange={handleInputChange}
            variant="outlined"
            fullWidth
            size="small"
            type="number"
            inputProps={{ min: 1, max: 4094 }}
            error={!!error}
            helperText={error || "Enter VLAN ID to search (1-4094)"}
            placeholder="e.g., 100"
          />

          <Divider />

          <Box display="flex" gap={1}>
            <Button
              variant="outlined"
              onClick={handleReset}
              fullWidth
              disabled={!idVlan}
            >
              Reset
            </Button>

            <Button
              variant="contained"
              type="submit"
              fullWidth
              disabled={!isValidVlanId(idVlan)}
            >
              Search
            </Button>
          </Box>

          {idVlan && (
            <Chip
              label={`Searching VLAN: ${idVlan}`}
              color="primary"
              variant="outlined"
              size="small"
            />
          )}
        </Box>
      </CardContent>
    </Card>
  );
}
