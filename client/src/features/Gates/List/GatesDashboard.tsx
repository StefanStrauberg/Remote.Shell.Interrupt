import { Grid2, Box } from "@mui/material";
import GateListPage from "./GateListPage";
import GateListFilter from "./GateListFilter";
import { useGatesQuery } from "../api/gatesQueries";
import { useState } from "react";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { DEFAULT_GATE_FILTERS } from "../api/gatesApi";
import { DEFAULT_PAGINATION } from "../../../lib/constants/pagination";
import RouterIcon from "@mui/icons-material/Router";
import PageHeader from "../../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../../app/shared/components/PageFeedback";

export default function GatesDashboard() {
  const [filters, setFilters] =
    useState<FilterDescriptor[]>(DEFAULT_GATE_FILTERS);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 12;
  const [orderBy] = useState<string>("ipAddress");
  const [orderByDescending] = useState<boolean>(false);

  const gatesQuery = useGatesQuery({
    pagination: { pageNumber, pageSize },
    filters,
    orderBy: { property: orderBy, descending: orderByDescending },
  });
  const gates = gatesQuery.data?.data ?? [];
  const pagination = gatesQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const { isPending, error } = gatesQuery;

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_GATE_FILTERS);
    setPageNumber(1);
  };

  if (isPending) {
    return <PageLoading message="Loading gates…" />;
  }

  if (error) {
    return <PageError title="Unable to load gates" error={error} />;
  }

  return (
    <Box>
      <PageHeader
        title="Gate management"
        description="Manage edge routers and their connection settings"
        icon={RouterIcon}
      />

      {gates.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
            <EmptyPage input="No gates found" />
          </Grid2>
          <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
            <GateListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={filters}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
            <GateListPage
              gates={gates}
              isPending={isPending}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>

          <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
            <GateListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={filters}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      )}
    </Box>
  );
}
