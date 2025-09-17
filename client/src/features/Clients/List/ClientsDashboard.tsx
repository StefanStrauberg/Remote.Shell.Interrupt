import { Grid2, Box, Typography, CircularProgress } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientListFilter from "./ClientListFilter";
import { useState } from "react";
import { useClients } from "../../../lib/hooks/useClients";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { DEFAULT_FILTERS_Clients } from "../../../lib/api/Clients/DEFAULT_FILTERS_Clients";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";
import GroupsIcon from "@mui/icons-material/Groups";

export default function ClientsDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_Clients
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy, setOrderBy] = useState<string>("name");
  const [orderByDescending, setOrderByDescending] = useState<boolean>(false);

  const pageSize = 12;
  const { clients, pagination, isLoadingClients, errorClients } = useClients(
    { pageNumber, pageSize },
    filters,
    { property: orderBy, descending: orderByDescending }
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1); // Reset to first page when filters change
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_FILTERS_Clients);
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

  // Show loading state
  if (isLoadingClients) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="400px"
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Loading clients...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (errorClients) {
    return (
      <Box>
        <Typography variant="h6" color="error" gutterBottom>
          Error loading clients
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {/* FIXED: Safe error message display */}
          {errorClients instanceof Error
            ? errorClients.message
            : "An unknown error occurred"}
        </Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" alignItems="center" mb={3}>
        <GroupsIcon sx={{ mr: 1, fontSize: 32, color: "primary.main" }} />
        <Typography variant="h4" component="h1" fontWeight="bold">
          Clients Dashboard
        </Typography>
      </Box>

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
            initialFilters={DEFAULT_FILTERS_Clients}
            onResetFilters={handleResetFilters}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
