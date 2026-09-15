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
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { FilterOperator } from "@/lib/types/Common/FilterOperator";
import { DEFAULT_USER_FILTERS } from "../api/usersApi";

type UsersListFilterProps = {
  onApplyFilters: (filters: FilterDescriptor[]) => void;
  initialFilters?: FilterDescriptor[];
  onResetFilters?: () => void;
};

export default function UsersListFilter({
  onApplyFilters,
  initialFilters = [],
  onResetFilters,
}: UsersListFilterProps) {
  const getInitialString = (property: string, defaultValue: string): string => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value : defaultValue;
  };

  const [email, setEmail] = useState<string>(getInitialString("Email", ""));
  const [isActive, setIsActive] = useState<string>(
    getInitialString("IsActive", "")
  );

  const createFilter = (
    property: string,
    operator: FilterOperator,
    value: string
  ): FilterDescriptor => ({
    PropertyPath: property,
    Operator: operator,
    Value: value,
  });

  const handleApply = () => {
    const normalizedEmail = email.trim();
    const filters: FilterDescriptor[] = [
      ...(normalizedEmail !== ""
        ? [createFilter("Email", FilterOperator.Contains, normalizedEmail)]
        : []),
      ...(isActive !== ""
        ? [createFilter("IsActive", FilterOperator.Equals, isActive)]
        : []),
    ];
    onApplyFilters(filters);
  };

  const handleReset = () => {
    setEmail("");
    setIsActive("");
    onApplyFilters(DEFAULT_USER_FILTERS);
    onResetFilters?.();
  };

  const handleActiveChange = (event: SelectChangeEvent<string>) => {
    setIsActive(event.target.value);
  };

  const hasActiveFilters = email.trim() !== "" || isActive !== "";

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
            label="Email contains"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            variant="outlined"
            fullWidth
            size="small"
          />

          <FormControl fullWidth size="small">
            <InputLabel>Status</InputLabel>
            <Select
              value={isActive}
              onChange={handleActiveChange}
              input={<OutlinedInput label="Status" />}
            >
              <MenuItem value="">
                <em>Any</em>
              </MenuItem>
              <MenuItem value="true">Active</MenuItem>
              <MenuItem value="false">Deactivated</MenuItem>
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
