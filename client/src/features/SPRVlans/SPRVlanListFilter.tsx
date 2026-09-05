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
  Chip,
} from "@mui/material";
import { useEffect, useState } from "react";
import { FilterList } from "@mui/icons-material";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { FilterOperator } from "../../lib/types/Common/FilterOperator";
import { DEFAULT_FILTERS_SPRVlans } from "../../lib/api/sprVlans/DEFAULT_FILTERS_SPRVlans";

type SPRVlanListFilterProps = {
  onApplyFilters: (filters: FilterDescriptor[]) => void;
  initialFilters?: FilterDescriptor[];
  onResetFilters?: () => void;
};

export default function SPRVlanListFilter({
  onApplyFilters,
  initialFilters = [],
  onResetFilters,
}: SPRVlanListFilterProps) {
  const getInitialNumber = (property: string, defaultValue: number): number => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? parseInt(filter.Value) || defaultValue : defaultValue;
  };

  const getInitialBoolean = (
    property: string,
    defaultValue: boolean
  ): boolean => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value === "true" : defaultValue;
  };

  const [idVlan, setIdVlan] = useState<number>(getInitialNumber("IdVlan", 0));
  const [idClient, setIdClient] = useState<number>(
    getInitialNumber("IdClient", 0)
  );
  const [useClient, setUseClient] = useState<boolean>(
    getInitialBoolean("UseClient", true)
  );
  const [useCOD, setUseCOD] = useState<boolean>(
    getInitialBoolean("UseCOD", false)
  );

  useEffect(() => {
    const currentVlanFilter = initialFilters.find(
      (f) => f.PropertyPath === "IdVlan"
    );
    const currentIdClientFilter = initialFilters.find(
      (f) => f.PropertyPath === "IdClient"
    );
    const currentUseClientFilter = initialFilters.find(
      (f) => f.PropertyPath === "UseClient"
    );
    const currentUseCODFilter = initialFilters.find(
      (f) => f.PropertyPath === "UseCOD"
    );

    setIdVlan(currentVlanFilter ? parseInt(currentVlanFilter.Value) || 0 : 0);
    setIdClient(
      currentIdClientFilter ? parseInt(currentIdClientFilter.Value) || 0 : 0
    );
    setUseClient(
      currentUseClientFilter ? currentUseClientFilter.Value === "true" : true
    );
    setUseCOD(
      currentUseCODFilter ? currentUseCODFilter.Value === "true" : false
    );
  }, [initialFilters]);

  const createFilter = (
    property: string,
    operator: FilterOperator,
    value: string | number | boolean
  ): FilterDescriptor => ({
    PropertyPath: property,
    Operator: operator,
    Value: String(value),
  });

  const handleApply = () => {
    const filters: FilterDescriptor[] = [
      createFilter("UseClient", FilterOperator.Equals, useClient),
      createFilter("UseCOD", FilterOperator.Equals, useCOD),
      ...(idVlan !== 0
        ? [createFilter("IdVlan", FilterOperator.Equals, idVlan)]
        : []),
      ...(idClient !== 0
        ? [createFilter("IdClient", FilterOperator.Equals, idClient)]
        : []),
    ];
    onApplyFilters(filters);
  };

  const handleReset = () => {
    setIdVlan(0);
    setIdClient(0);
    setUseClient(true);
    setUseCOD(false);
    onApplyFilters(DEFAULT_FILTERS_SPRVlans);
    onResetFilters?.();
  };

  const handleNumberChange =
    (setter: React.Dispatch<React.SetStateAction<number>>) =>
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const value = e.target.value;
      if (value === "" || /^\d*$/.test(value)) {
        setter(parseInt(value) || 0);
      }
    };

  const hasActiveFilters =
    idVlan !== 0 || idClient !== 0 || !useClient || useCOD;

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
            label="VLAN ID"
            value={idVlan === 0 ? "" : idVlan.toString()}
            onChange={handleNumberChange(setIdVlan)}
            variant="outlined"
            fullWidth
            size="small"
            type="number"
            inputProps={{ min: 0 }}
          />

          <TextField
            label="Client ID"
            value={idClient === 0 ? "" : idClient.toString()}
            onChange={handleNumberChange(setIdClient)}
            variant="outlined"
            fullWidth
            size="small"
            type="number"
            inputProps={{ min: 0 }}
          />

          <Divider />

          <Box>
            <FormControlLabel
              control={
                <Checkbox
                  checked={useClient}
                  onChange={(e) => setUseClient(e.target.checked)}
                  color="primary"
                />
              }
              label="Used by Client"
            />

            <FormControlLabel
              control={
                <Checkbox
                  checked={useCOD}
                  onChange={(e) => setUseCOD(e.target.checked)}
                  color="primary"
                />
              }
              label="Used by COD"
            />
          </Box>

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
