import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Checkbox,
  Divider,
  FormControlLabel,
  Grid2,
  TextField,
} from "@mui/material";
import { SPRVlanFilter } from "../../lib/types/SPRVlanFilter";
import { useState } from "react";
import { FilterList } from "@mui/icons-material";

type Props = {
  onApplyFilters: (filters: SPRVlanFilter) => void;
};

export default function SPRVlanListFilter({ onApplyFilters }: Props) {
  const [idVlan, setIdVlan] = useState<number>(0);
  const [idClient, setIdClient] = useState<number>(0);
  const [useClient, setUseClient] = useState<boolean>(true);
  const [useCOD, setUseCOD] = useState<boolean>(false);

  const handleApplyClick = () => {
    const filters: SPRVlanFilter = {};
    if (idVlan) filters.IdVlan = { op: "==", value: idVlan };
    if (idClient) filters.IdClient = { op: "==", value: idClient };
    if (useClient) filters.UseClient = { op: "==", value: useClient };
    if (useCOD) filters.UseCOD = { op: "==", value: useCOD };
    onApplyFilters(filters);
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
            onChange={(e) => {
              const newValue = e.target.value;
              if (!isNaN(Number(newValue))) {
                setIdVlan(Number(newValue)); // Преобразуем введенное значение в число
              }
            }}
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
            onChange={(e) => {
              const newValue = e.target.value;
              if (!isNaN(Number(newValue))) {
                setIdClient(Number(newValue)); // Преобразуем введенное значение в число
              }
            }}
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
          <Button
            variant="contained"
            color="success"
            onClick={handleApplyClick}
          >
            Применить
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
