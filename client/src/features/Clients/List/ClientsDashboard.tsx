import { Grid2 } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientListFilter from "./ClientListFilter";
import { useState } from "react";
import { useClients } from "../../../lib/hooks/useClients";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { DEFAULT_FILTERS_Clients } from "../../../lib/api/Clients/DEFAULT_FILTERS_Clients";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";

export default function ClientsDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_Clients
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy, setOrderBy] = useState<string>("name");
  const [orderByDescending, setOrderByDescending] = useState<boolean>(false);

  const pageSize = 10;
  const { clients, pagination, isLoadingClients } = useClients(
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

  return (
    <>
      {clients?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input={"Клиенты не найдены"} />
          </Grid2>
          <Grid2 size={3}>
            <ClientListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={DEFAULT_FILTERS_Clients}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <ClientListPage
              clients={clients}
              isPending={isLoadingClients}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
              orderBy={orderBy}
              orderByDescending={orderByDescending}
              onSort={handleSort}
            />
          </Grid2>
          <Grid2 size={3}>
            <ClientListFilter
              onApplyFilters={handleApplyFilters}
              initialFilters={DEFAULT_FILTERS_Clients}
              onResetFilters={handleResetFilters}
            />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
