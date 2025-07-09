import { Grid2 } from "@mui/material";
import SPRVlanListPage from "./SPRVlanListPage";
import { useState } from "react";
import { useSPRVlans } from "../../lib/hooks/useSPRVlans";
import SPRVlanListFilter from "./SPRVlanListFilter";
import EmptyPage from "../../app/shared/components/EmptyPage";

export default function SPRVlansDashboard() {
  const [filters, setFilters] = useState<SPRVlanFilter>();

  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 15;

  // Hook for fetching data
  const { sprVlans, pagination, isLoading } = useSPRVlans(
    { pageNumber, pageSize },
    filters
  );

  const handleApplyFilters = (newFilters: SPRVlanFilter) => {
    setFilters(newFilters);
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
            <SPRVlanListFilter onApplyFilters={handleApplyFilters} />
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
            <SPRVlanListFilter onApplyFilters={handleApplyFilters} />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
