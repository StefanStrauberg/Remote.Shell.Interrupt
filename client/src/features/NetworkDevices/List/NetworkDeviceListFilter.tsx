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
import { FilterList } from "@mui/icons-material";
import { useState } from "react";
import { typeOfNetworkDeviceOptions } from "../../../lib/types/Common/typeOfNetworkDeviceOptions";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { FilterOperator } from "../../../lib/types/Common/FilterOperator";
import { DEFAULT_FILTERS_NetworkDevices } from "../../../lib/api/networkDevices/DEFAULT_FILTERS_NetworkDevices";

const MenuProps = {
  PaperProps: {
    style: {
      maxHeight: 250,
      width: 250,
    },
  },
};

type NetworkDeviceListFilterProps = {
  onApplyFilters: (filters: FilterDescriptor[]) => void;
  initialFilters?: FilterDescriptor[];
  onResetFilters?: () => void;
};

export default function NetworkDeviceListFilter({
  onApplyFilters,
  initialFilters = [],
  onResetFilters,
}: NetworkDeviceListFilterProps) {
  const getInitialString = (property: string, defaultValue: string): string => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value : defaultValue;
  };

  const [NetworkDeviceName, setNetworkDeviceName] = useState<string>(
    getInitialString("NetworkDeviceName", "")
  );
  const [Host, setHost] = useState<string>(getInitialString("Host", ""));
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
      ...(NetworkDeviceName !== ""
        ? [
            createFilter(
              "NetworkDeviceName",
              FilterOperator.Contains,
              NetworkDeviceName
            ),
          ]
        : []),
      ...(Host !== ""
        ? [createFilter("Host", FilterOperator.Equals, Host)]
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
    setNetworkDeviceName("");
    setHost("");
    setTypeOfNetworkDevice("");
    onApplyFilters(DEFAULT_FILTERS_NetworkDevices);
    onResetFilters?.();
  };

  const handleChange = (event: SelectChangeEvent<string>) => {
    setTypeOfNetworkDevice(event.target.value as string);
  };

  const hasActiveFilters =
    NetworkDeviceName !== "" || Host !== "" || typeOfNetworkDevice !== "";

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
            label="Device Name"
            value={NetworkDeviceName}
            onChange={(e) => setNetworkDeviceName(e.target.value)}
            variant="outlined"
            fullWidth
            size="small"
          />

          <TextField
            label="IP Address"
            value={Host}
            onChange={(e) => setHost(e.target.value)}
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
