import { Grid2, Box } from "@mui/material";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import NetworkDeviceListPage from "./NetworkDeviceListPage";
import { useNetworkDevicesQuery } from "../api/networkDevicesQueries";
import { useState } from "react";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import { DEFAULT_NETWORK_DEVICE_FILTERS } from "../api/networkDevicesApi";
import NetworkDeviceListFilter from "./NetworkDeviceListFilter";
import RouterIcon from "@mui/icons-material/Router";
import { DEFAULT_PAGINATION } from "../../../lib/constants/pagination";
import PageHeader from "../../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../../app/shared/components/PageFeedback";

export default function NetworkDeviceDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_NETWORK_DEVICE_FILTERS
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy] = useState<string>("host");
  const [orderByDescending] = useState<boolean>(false);
  const pageSize = 12;

  const networkDevicesQuery = useNetworkDevicesQuery({
    pagination: { pageNumber, pageSize },
    filters,
    orderBy: { property: orderBy, descending: orderByDescending },
  });
  const networkDevices = networkDevicesQuery.data?.data ?? [];
  const pagination = networkDevicesQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const { isPending, error } = networkDevicesQuery;

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_NETWORK_DEVICE_FILTERS);
    setPageNumber(1);
  };

  if (isPending) {
    return <PageLoading message="Loading network devices…" />;
  }

  if (error) {
    return <PageError title="Unable to load network devices" error={error} />;
  }

  return (
    <Box>
      <PageHeader
        title="Network devices"
        description="Monitor gateways, switches, and discovered equipment"
        icon={RouterIcon}
      />

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
            initialFilters={filters}
            onResetFilters={handleResetFilters}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
