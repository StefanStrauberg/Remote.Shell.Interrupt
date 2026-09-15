import { Grid2, Box } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientListFilter from "./ClientListFilter";
import { useState } from "react";
import { useClientsQuery } from "../api/clientsQueries";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { DEFAULT_CLIENT_FILTERS } from "../api/clientsApi";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import GroupsIcon from "@mui/icons-material/Groups";
import { DEFAULT_PAGINATION } from "../../../lib/constants/pagination";
import PageHeader from "../../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../../app/shared/components/PageFeedback";

export default function ClientsDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_CLIENT_FILTERS
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy, setOrderBy] = useState<string>("name");
  const [orderByDescending, setOrderByDescending] = useState<boolean>(false);

  const pageSize = 12;
  const clientsQuery = useClientsQuery({
    pagination: { pageNumber, pageSize },
    filters,
    orderBy: { property: orderBy, descending: orderByDescending },
  });
  const clients = clientsQuery.data?.data ?? [];
  const pagination = clientsQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const isLoadingClients = clientsQuery.isPending;
  const errorClients = clientsQuery.error;

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1); // Reset to first page when filters change
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_CLIENT_FILTERS);
    setPageNumber(1);
  };

  const handleSort = (property: string) => {
    if (orderBy === property) {
      // Toggle direction if same property is clicked
      setOrderByDescending(!orderByDescending);
    } else {
      // New property, default to ascending
      setOrderBy(property);
      setOrderByDescending(false);
    }
    setPageNumber(1); // Reset to first page when sorting changes
  };

  if (isLoadingClients) {
    return <PageLoading message="Loading clients…" />;
  }

  if (errorClients) {
    return <PageError title="Unable to load clients" error={errorClients} />;
  }

  return (
    <Box>
      <PageHeader
        title="Clients"
        description="Browse and filter synchronized customer records"
        icon={GroupsIcon}
      />

      <Grid2 container spacing={3}>
        <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
          {clients?.length === 0 ? (
            <EmptyPage
              input={"No clients found"}
              description="Try adjusting your filters or search criteria"
            />
          ) : (
            <ClientListPage
              clients={clients || []}
              isPending={isLoadingClients}
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
          <ClientListFilter
            onApplyFilters={handleApplyFilters}
            initialFilters={filters}
            onResetFilters={handleResetFilters}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
