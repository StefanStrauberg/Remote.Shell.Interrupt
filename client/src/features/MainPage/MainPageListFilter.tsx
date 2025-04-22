import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Divider,
  TextField,
} from "@mui/material";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import { useState } from "react";

type Props = {
  onApplyFilters: (filters: RouterFilter) => void;
};

export default function MainPageListFilter({ onApplyFilters }: Props) {
  const [idVlan, setIdVlan] = useState<number>(0);

  const handleApplyClick = () => {
    const filters: RouterFilter = {};
    if (idVlan) filters.IdVlan = { op: "==", value: idVlan };
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
          <Divider />
          <ButtonGroup>
            <Button
              variant="contained"
              color="info"
              onClick={() => {
                setIdVlan(0);
              }}
            >
              Сбросить
            </Button>
            <Button
              variant="contained"
              color="success"
              onClick={handleApplyClick}
            >
              Применить
            </Button>
          </ButtonGroup>
        </Box>
      </CardContent>
    </Card>
  );
}
