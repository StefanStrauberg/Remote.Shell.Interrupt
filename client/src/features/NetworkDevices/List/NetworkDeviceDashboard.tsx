import { Grid2, Box, Typography, CircularProgress } from "@mui/material";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import NetworkDeviceListPage from "./NetworkDeviceListPage";
import { useNetworkDevices } from "../../../lib/hooks/useNetworkDevices";
import { useState } from "react";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { DEFAULT_FILTERS_NetworkDevices } from "../../../lib/api/networkDevices/DEFAULT_FILTERS_NetworkDevices";
import NetworkDeviceListFilter from "./NetworkDeviceListFilter";
import RouterIcon from "@mui/icons-material/Router";

export default function NetworkDeviceDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_NetworkDevices
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy] = useState<string>("host");
  const [orderByDescending] = useState<boolean>(false);
  const pageSize = 12;

  const { networkDevices, pagination, isPending, error } = useNetworkDevices(
    { pageNumber, pageSize },
    filters,
    { property: orderBy, descending: orderByDescending }
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_FILTERS_NetworkDevices);
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
          Loading network devices...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (error) {
    return (
      <Box>
        <Typography variant="h6" color="error" gutterBottom>
          Error loading network devices
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
          Network Devices Dashboard
        </Typography>
      </Box>

      <Grid2 container spacing={3}>
        <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
          {networkDevices?.length === 0 ? (
            <EmptyPage input="Network devices not found" />
          ) : (
            <NetworkDeviceListPage
              networkDevices={networkDevices || []}
              isLoadingNetworkDevices={isPending}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          )}
        </Grid2>

        <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
          <NetworkDeviceListFilter
            onApplyFilters={handleApplyFilters}
            initialFilters={DEFAULT_FILTERS_NetworkDevices}
            onResetFilters={handleResetFilters}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
