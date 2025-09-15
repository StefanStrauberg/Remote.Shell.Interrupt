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
  // Parse initial filters
  const getInitialBoolean = (
    property: string,
    defaultValue: boolean
  ): boolean => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value === "true" : defaultValue;
  };

  const getInitialNumber = (property: string, defaultValue: number): number => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? parseInt(filter.Value) || defaultValue : defaultValue;
  };

  // Form state
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
            label="Id влана"
            value={idVlan.toString() || ""}
            onChange={handleNumberChange(setIdVlan)}
            variant="outlined"
            fullWidth
            type="number" // Устанавливаем тип поля как "number"
            inputProps={{ min: 0 }} // Ограничение ввода только положительных чисел (опционально)
            error={isNaN(idVlan)} // Показывает ошибку, если значение некорректное
            helperText={isNaN(idVlan) ? "Введите корректное число" : ""} // Текст подсказки
          />
          <TextField
            label="Id клиента"
            value={idClient.toString() || ""}
            onChange={handleNumberChange(setIdClient)}
            variant="outlined"
            fullWidth
            type="number" // Устанавливаем тип поля как "number"
            inputProps={{ min: 0 }} // Ограничение ввода только положительных чисел (опционально)
            error={isNaN(idClient)} // Показывает ошибку, если значение некорректное
            helperText={isNaN(idClient) ? "Введите корректное число" : ""} // Текст подсказки
          />
          <Divider />
          <Grid2 container spacing={1}>
            <Grid2 size={6}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={useClient}
                    onChange={(e) => setUseClient(e.target.checked)}
                  />
                }
                label="Использ. клиент"
              />
            </Grid2>
            <Grid2 size={6}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={useCOD}
                    onChange={(e) => setUseCOD(e.target.checked)}
                  />
                }
                label="Использ. COD"
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
