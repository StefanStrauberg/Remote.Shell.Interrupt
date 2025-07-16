import { Grid2 } from "@mui/material";
import GateListPage from "./GateListPage";
import GateListFilter from "./GateListFilter";
import { useGates } from "../../../lib/hooks/useGates";
import { useState } from "react";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { DEFAULT_FILTERS_Gates } from "../../../lib/api/gates/DefaultFiltersGates";

export default function GatesDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_Gates
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 10;
  const { gates, pagination, isPending } = useGates(
    { pageNumber, pageSize },
    filters
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1); // сбросить страницу
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_FILTERS_Gates);
    setPageNumber(1);
  };

  return (
    <>
      {gates?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input="Маршрутизаторы не найдены" />
          </Grid2>
          <Grid2 size={3}>
            <GateListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={DEFAULT_FILTERS_Gates}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <GateListPage
              gates={gates}
              isPending={isPending}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>
          <Grid2 size={3}>
            <GateListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={DEFAULT_FILTERS_Gates}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
