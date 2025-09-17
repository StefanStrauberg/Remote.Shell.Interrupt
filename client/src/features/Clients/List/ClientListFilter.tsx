import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Checkbox,
  Divider,
  FormControlLabel,
  TextField,
  FormGroup,
  Chip,
} from "@mui/material";
import { useState } from "react";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { FilterOperator } from "../../../lib/types/Common/FilterOperator";
import { DEFAULT_FILTERS_Clients } from "../../../lib/api/Clients/DEFAULT_FILTERS_Clients";

type ClientListFilterProps = {
  onApplyFilters: (filters: FilterDescriptor[]) => void;
  initialFilters?: FilterDescriptor[];
  onResetFilters?: () => void;
};

export default function ClientListFilter({
  onApplyFilters,
  initialFilters = [],
  onResetFilters,
}: ClientListFilterProps) {
  const getInitialBoolean = (
    property: string,
    defaultValue: boolean
  ): boolean => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value === "true" : defaultValue;
  };

  const getInitialString = (property: string, defaultValue: string): string => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value : defaultValue;
  };

  const [name, setName] = useState<string>(getInitialString("Name", ""));
  const [nrDogovor, setNrDogovor] = useState<string>(
    getInitialString("NrDogovor", "")
  );
  const [working, setWorking] = useState<boolean>(
    getInitialBoolean("Working", true)
  );
  const [antiDDOS, setAntiDDOS] = useState<boolean>(
    getInitialBoolean("AntiDDOS", false)
  );

  const createFilter = (
    property: string,
    operator: FilterOperator,
    value: string | boolean
  ): FilterDescriptor => ({
    PropertyPath: property,
    Operator: operator,
    Value: String(value),
  });

  const handleApply = () => {
    const filters: FilterDescriptor[] = [
      createFilter("Working", FilterOperator.Equals, working),
      createFilter("AntiDDOS", FilterOperator.Equals, antiDDOS),
      ...(name !== ""
        ? [createFilter("Name", FilterOperator.Contains, name)]
        : []),
      ...(nrDogovor !== ""
        ? [createFilter("NrDogovor", FilterOperator.Contains, nrDogovor)]
        : []),
    ];
    onApplyFilters(filters);
  };

  const handleReset = () => {
    setName("");
    setNrDogovor("");
    setWorking(true);
    setAntiDDOS(false);
    onApplyFilters(DEFAULT_FILTERS_Clients);
    onResetFilters?.();
  };

  const hasActiveFilters =
    name !== "" || nrDogovor !== "" || !working || antiDDOS;

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
            <span>Filters</span>
            {hasActiveFilters && (
              <Chip
                label="Active"
                size="small"
                color="primary"
                sx={{ ml: 1 }}
              />
            )}
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
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            label="Organization Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            variant="outlined"
            fullWidth
            size="small"
          />

          <TextField
            label="Contract Number"
            value={nrDogovor}
            onChange={(e) => setNrDogovor(e.target.value)}
            variant="outlined"
            fullWidth
            size="small"
          />

          <Divider />

          <FormGroup>
            <FormControlLabel
              control={
                <Checkbox
                  checked={working}
                  onChange={(e) => setWorking(e.target.checked)}
                  color="primary"
                />
              }
              label="Active Clients"
            />

            <FormControlLabel
              control={
                <Checkbox
                  checked={antiDDOS}
                  onChange={(e) => setAntiDDOS(e.target.checked)}
                  color="primary"
                />
              }
              label="AntiDDOS Enabled"
            />
          </FormGroup>

          <Divider />

          <Box display="flex" gap={1}>
            <Button
              variant="outlined"
              onClick={handleReset}
              fullWidth
              disabled={!hasActiveFilters}
            >
              Reset
            </Button>

            <Button variant="contained" onClick={handleApply} fullWidth>
              Apply
            </Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
