import { Grid2 } from "@mui/material";
import MainPageListFilter from "./MainPageListFilter";
import MainPageList from "./MainPageList";
import { useState } from "react";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import { useRouters } from "../../lib/hooks/useRouters";

export default function MainPageDashboard() {
  const [filters, setFilters] = useState<RouterFilter>({});

  const { compoundObject } = useRouters(filters);

  return (
    <Grid2 container spacing={3}>
      <Grid2 size={9}>
        <MainPageList />
      </Grid2>
      <Grid2 size={3}>
        <MainPageListFilter />
      </Grid2>
    </Grid2>
  );
}
