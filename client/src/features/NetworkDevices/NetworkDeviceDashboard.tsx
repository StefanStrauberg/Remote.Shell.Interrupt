import { Grid2 } from "@mui/material";
import EmptyPage from "../../app/shared/components/EmptyPage";
import NetworkDeviceListPage from "./NetworkDeviceListPage";
import { useNetworkDevices } from "../../lib/hooks/useNetworkDevices";
import { useState } from "react";
import NetworkDeviceListFilter from "./NetworkDeviceListFilter";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { DEFAULT_FILTERS_NetworkDevices } from "../../lib/api/networkDevices/DEFAULT_FILTERS_NetworkDevices";

export default function NetworkDeviceDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_NetworkDevices
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 10;

  const { networkDevices, pagination, isPending } = useNetworkDevices(
    { pageNumber, pageSize },
    filters
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
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
              isPending={isPending}
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
