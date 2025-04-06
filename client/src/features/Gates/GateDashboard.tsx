import { Grid2 } from "@mui/material";
import GateListPage from "./GateListPage";
import GateListFilter from "./GateListFilter";
import { useGates } from "../../lib/hooks/useGates";
import GatesEmptyCard from "./GatesEmptyCard";
import { GateFilter } from "../../lib/types/GateFilter";
import { useState } from "react";

export default function GateDashboard() {
  const [filters, setFilters] = useState<GateFilter>({});
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 10;

  const { gates, pagination, isPending } = useGates(
    pageNumber,
    pageSize,
    filters
  );

  const handleApplyFilters = (newFilters: GateFilter) => {
    setFilters(newFilters);
    setPageNumber(1); // сбросить страницу
  };

  return (
    <>
      {gates?.length === 0 ? (
        <GatesEmptyCard onApplyFilters={handleApplyFilters} />
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
            <GateListFilter onApplyFilters={handleApplyFilters} />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
