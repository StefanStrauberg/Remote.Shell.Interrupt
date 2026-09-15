import { Grid2, Box } from "@mui/material";
import SPRVlanListPage from "./SPRVlanListPage";
import { useState } from "react";
import { useSprVlansQuery } from "./api/sprVlansQueries";
import EmptyPage from "../../app/shared/components/EmptyPage";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import SPRVlanListFilter from "./SPRVlanListFilter";
import { DEFAULT_SPR_VLAN_FILTERS } from "./api/sprVlansApi";
import VpnLockIcon from "@mui/icons-material/VpnLock";
import { DEFAULT_PAGINATION } from "../../lib/constants/pagination";
import PageHeader from "../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../app/shared/components/PageFeedback";

export default function SPRVlansDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_SPR_VLAN_FILTERS
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy, setOrderBy] = useState<string>("idVlan");
  const [orderByDescending, setOrderByDescending] = useState<boolean>(false);
  const pageSize = 15;

  const sprVlansQuery = useSprVlansQuery({
    pagination: { pageNumber, pageSize },
    filters,
    orderBy: { property: orderBy, descending: orderByDescending },
  });
  const sprVlans = sprVlansQuery.data?.data ?? [];
  const pagination = sprVlansQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const isLoading = sprVlansQuery.isPending;
  const error = sprVlansQuery.error;

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_SPR_VLAN_FILTERS);
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

  if (isLoading) {
    return <PageLoading message="Loading VLANs…" />;
  }

  if (error) {
    return <PageError title="Unable to load VLANs" error={error} />;
  }

  return (
    <Box>
      <PageHeader
        title="VLAN management"
        description="Review service VLAN assignments and customer links"
        icon={VpnLockIcon}
      />

      <Grid2 container spacing={3}>
        <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
          {sprVlans.length === 0 && !isLoading ? (
            <EmptyPage input="No VLANs found" />
          ) : (
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
          )}
        </Grid2>
        <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
          <SPRVlanListFilter
            onApplyFilters={handleApplyFilters}
            initialFilters={filters}
            onResetFilters={handleResetFilters}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
