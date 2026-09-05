import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  FormControl,
  InputLabel,
  MenuItem,
  OutlinedInput,
  TextField,
  Chip,
  Select,
  SelectChangeEvent,
} from "@mui/material";
import { useState } from "react";
import { typeOfNetworkDeviceOptions } from "../../../lib/types/Common/typeOfNetworkDeviceOptions";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { FilterOperator } from "../../../lib/types/Common/FilterOperator";
import { DEFAULT_FILTERS_Gates } from "../../../lib/api/gates/DEFAULT_FILTERS_Gates";

const MenuProps = {
  PaperProps: {
    style: {
      maxHeight: 250,
      width: 250,
    },
  },
};

type GateListFilterProps = {
  onApplyFilters: (filters: FilterDescriptor[]) => void;
  initialFilters?: FilterDescriptor[];
  onResetFilters?: () => void;
};

export default function GateListFilter({
  onApplyFilters,
  initialFilters = [],
  onResetFilters,
}: GateListFilterProps) {
  const getInitialString = (property: string, defaultValue: string): string => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value : defaultValue;
  };

  const [name, setName] = useState<string>(getInitialString("Name", ""));
  const [ipAddress, setIpAddress] = useState<string>(
    getInitialString("IpAddress", "")
  );
  const [typeOfNetworkDevice, setTypeOfNetworkDevice] = useState<string>(
    getInitialString("TypeOfNetworkDevice", "")
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
      ...(name !== ""
        ? [createFilter("Name", FilterOperator.Contains, name)]
        : []),
      ...(ipAddress !== ""
        ? [createFilter("IpAddress", FilterOperator.Equals, ipAddress)]
        : []),
      ...(typeOfNetworkDevice !== ""
        ? [
            createFilter(
              "TypeOfNetworkDevice",
              FilterOperator.Equals,
              typeOfNetworkDevice
            ),
          ]
        : []),
    ];
    onApplyFilters(filters);
  };

  const handleReset = () => {
    setName("");
    setIpAddress("");
    setTypeOfNetworkDevice("");
    onApplyFilters(DEFAULT_FILTERS_Gates);
    onResetFilters?.();
  };

  const handleChange = (event: SelectChangeEvent<string>) => {
    setTypeOfNetworkDevice(event.target.value);
  };

  const hasActiveFilters =
    name !== "" || ipAddress !== "" || typeOfNetworkDevice !== "";

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
            label="Gate Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            variant="outlined"
            fullWidth
            size="small"
          />

          <TextField
            label="IP Address"
            value={ipAddress}
            onChange={(e) => setIpAddress(e.target.value)}
            variant="outlined"
            fullWidth
            size="small"
          />

          <FormControl fullWidth size="small">
            <InputLabel>Device Type</InputLabel>
            <Select
              value={typeOfNetworkDevice}
              onChange={handleChange}
              input={<OutlinedInput label="Device Type" />}
              MenuProps={MenuProps}
            >
              <MenuItem value="">
                <em>None</em>
              </MenuItem>
              {typeOfNetworkDeviceOptions.map((typeOfND) => (
                <MenuItem key={typeOfND.value} value={typeOfND.value}>
                  {typeOfND.value}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

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
