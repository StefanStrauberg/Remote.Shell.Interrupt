import { Grid2 } from "@mui/material";
import EmptyPage from "../../app/shared/components/EmptyPage";
import NetworkDeviceListPage from "./NetworkDeviceListPage";
import { useNetworkDevices } from "../../lib/hooks/useNetworkDevices";
import { useState } from "react";
import NetworkDeviceListFilter from "./NetworkDeviceListFilter";

export default function NetworkDeviceDashboard() {
  const [filters, setFilters] = useState<NetworkDeviceFilter>({});
  // Manage local state for pagination

  const [pageNumber, setPageNumber] = useState<number>(1); // TablePagination uses zero-based index
  const pageSize = 10; // Default page size

  const { networkDevices, isLoadingNetworkDevices, pagination } =
    useNetworkDevices(pageNumber, pageSize, filters);

  const handleApplyFilters = (newFilters: NetworkDeviceFilter) => {
    setFilters(newFilters);
    setPageNumber(1); // сбросить страницу
  };

  return (
    <>
      {networkDevices?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input="Шлюзы не найдены" />
          </Grid2>
          <Grid2 size={3}>
            <NetworkDeviceListFilter onApplyFilters={handleApplyFilters} />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <NetworkDeviceListPage
              networkDevices={networkDevices}
              isLoadingNetworkDevices={isLoadingNetworkDevices}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>
          <Grid2 size={3}>
            <NetworkDeviceListFilter onApplyFilters={handleApplyFilters} />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
