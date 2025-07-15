import { Grid2 } from "@mui/material";
import SPRVlanListPage from "./SPRVlanListPage";
import { useState } from "react";
import { useSPRVlans } from "../../lib/hooks/useSPRVlans";
import EmptyPage from "../../app/shared/components/EmptyPage";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import SPRVlanListFilter from "./SPRVlanListFilter";
import { DEFAULT_FILTERS_SPRVlans } from "../../lib/api/sprVlans/DefaultFiltersSPRVlans";

export default function SPRVlansDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_SPRVlans
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 15;
  const { sprVlans, pagination, isLoading } = useSPRVlans(
    { pageNumber, pageSize },
    filters
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1); // Reset to first page when filters change
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_FILTERS_SPRVlans);
    setPageNumber(1);
  };

  return (
    <>
      {sprVlans?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input="Вланы не найдены" />
          </Grid2>
          <Grid2 size={3}>
            <SPRVlanListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={DEFAULT_FILTERS_SPRVlans}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <SPRVlanListPage
              sprVlans={sprVlans}
              isPending={isLoading}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>
          <Grid2 size={3}>
            <SPRVlanListFilter
              onApplyFilters={(newFilters) => {
                setFilters(newFilters);
                setPageNumber(1);
              }}
            />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
