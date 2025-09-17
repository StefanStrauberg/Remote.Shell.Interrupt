import { Grid2, Box, Typography, CircularProgress } from "@mui/material";
import SPRVlanListPage from "./SPRVlanListPage";
import { useState } from "react";
import { useSPRVlans } from "../../lib/hooks/useSPRVlans";
import EmptyPage from "../../app/shared/components/EmptyPage";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import SPRVlanListFilter from "./SPRVlanListFilter";
import { DEFAULT_FILTERS_SPRVlans } from "../../lib/api/sprVlans/DEFAULT_FILTERS_SPRVlans";
import VpnLockIcon from "@mui/icons-material/VpnLock";

export default function SPRVlansDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_SPRVlans
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy, setOrderBy] = useState<string>("idVlan");
  const [orderByDescending, setOrderByDescending] = useState<boolean>(false);
  const pageSize = 15;

  const { sprVlans, pagination, isLoading, error } = useSPRVlans(
    { pageNumber, pageSize },
    filters,
    { property: orderBy, descending: orderByDescending }
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_FILTERS_SPRVlans);
    setPageNumber(1);
  };

  const handleSort = (property: string) => {
    if (orderBy === property) {
      setOrderByDescending(!orderByDescending);
    } else {
      setOrderBy(property);
      setOrderByDescending(false);
    }
    setPageNumber(1);
  };

  // Show loading state
  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="400px"
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Loading VLANs...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (error) {
    return (
      <Box>
        <Typography variant="h6" color="error" gutterBottom>
          Error loading VLANs
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
        <VpnLockIcon sx={{ mr: 1, fontSize: 32, color: "primary.main" }} />
        <Typography variant="h4" component="h1" fontWeight="bold">
          VLAN Management
        </Typography>
      </Box>

      {sprVlans.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input="No VLANs found" />
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
          <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
            <SPRVlanListPage
              sprVlans={sprVlans}
              isPending={isLoading}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
              orderBy={orderBy}
              orderByDescending={orderByDescending}
              onSort={handleSort}
            />
          </Grid2>

          <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
            <SPRVlanListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={DEFAULT_FILTERS_SPRVlans}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      )}
    </Box>
  );
}
