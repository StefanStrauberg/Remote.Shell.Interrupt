import { Grid2 } from "@mui/material";
import MainPageListFilter from "./MainPageListFilter";
import MainPageList from "./MainPageList";
import { useState } from "react";
import { useRouters } from "../../lib/hooks/useRouters";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";

export default function MainPageDashboard() {
  const [filters, setFilters] = useState<RouterFilter>({});
  const [isEnabled, setEnabled] = useState(false);

  const { compoundObject, resetCache } = useRouters(filters, isEnabled);

  const handleApplyFilters = (newFilters: RouterFilter) => {
    setFilters(newFilters);
    setEnabled(false); // сбрасываем флаг активации до нажатия кнопки
  };

  const handleSearch = () => {
    resetCache(); // очищаем кэш перед запросом
    setEnabled(true); // активируем запрос
  };

  return (
    <Grid2 container spacing={3}>
      <Grid2 size={9}>
        <MainPageList data={compoundObject} />
      </Grid2>
      <Grid2 size={3}>
        <MainPageListFilter
          onApplyFilters={handleApplyFilters}
          onSearch={handleSearch}
        />
      </Grid2>
    </Grid2>
  );
}
