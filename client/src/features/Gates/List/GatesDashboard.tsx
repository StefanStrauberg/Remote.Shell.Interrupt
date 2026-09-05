import { Grid2, Box, Typography, CircularProgress } from "@mui/material";
import GateListPage from "./GateListPage";
import GateListFilter from "./GateListFilter";
import { useGates } from "../../../lib/hooks/useGates";
import { useState } from "react";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { DEFAULT_FILTERS_Gates } from "../../../lib/api/gates/DEFAULT_FILTERS_Gates";
import RouterIcon from "@mui/icons-material/Router";

export default function GatesDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_Gates
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 12;
  const [orderBy] = useState<string>("ipAddress");
  const [orderByDescending] = useState<boolean>(false);

  const { gates, pagination, isPending, error } = useGates(
    { pageNumber, pageSize },
    filters,
    { property: orderBy, descending: orderByDescending }
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_FILTERS_Gates);
    setPageNumber(1);
  };

  // Show loading state
  if (isPending) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="400px"
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Loading gates...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (error) {
    return (
      <Box>
        <Typography variant="h6" color="error" gutterBottom>
          Error loading gates
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {error instanceof Error ? error.message : "An unknown error occurred"}
        </Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" alignItems="center" mb={3}>
        <RouterIcon sx={{ mr: 1, fontSize: 32, color: "primary.main" }} />
        <Typography variant="h4" component="h1" fontWeight="bold">
          Gate Management
        </Typography>
      </Box>

      {gates.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input="No gates found" />
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
              initialFilters={DEFAULT_FILTERS_Gates}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      )}
    </Box>
  );
}
