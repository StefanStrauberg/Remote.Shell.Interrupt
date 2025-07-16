import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Checkbox,
  Divider,
  FormControlLabel,
  Grid2,
  TextField,
} from "@mui/material";
import { useState } from "react";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { FilterOperator } from "../../../lib/types/Common/FilterOperator";
import { DEFAULT_FILTERS_Clients } from "../../../lib/api/Clients/DefaultFiltersClients";

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

  return (
    <Card
      sx={{
        borderRadius: 4,
        boxShadow: 3,
        bgcolor: "background.default",
        fontSize: 18,
        overflow: "hidden",
      }}
    >
      <CardHeader
        title="Фильтры"
        avatar={<FilterList color="primary" />}
        sx={{ bgcolor: "primary.light", color: "white", p: 2 }}
      />
      <CardContent>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            label="Название организации"
            value={name}
            onChange={(e) => setName(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <TextField
            label="Номер договора"
            value={nrDogovor}
            onChange={(e) => setNrDogovor(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <Divider />
          <Grid2 container spacing={1}>
            <Grid2 size={6}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={working}
                    onChange={(e) => setWorking(e.target.checked)}
                  />
                }
                label="Working"
              />
            </Grid2>
            <Grid2 size={6}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={antiDDOS}
                    onChange={(e) => setAntiDDOS(e.target.checked)}
                  />
                }
                label="AntiDDOS"
              />
            </Grid2>
          </Grid2>
          <Divider />
          <ButtonGroup>
            <Button variant="contained" color="info" onClick={handleReset}>
              Сбросить
            </Button>
            <Button variant="contained" color="success" onClick={handleApply}>
              Применить
            </Button>
          </ButtonGroup>
        </Box>
      </CardContent>
    </Card>
  );
}
