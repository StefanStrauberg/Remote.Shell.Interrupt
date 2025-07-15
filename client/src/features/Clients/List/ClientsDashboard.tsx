import { Grid2 } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientListFilter from "./ClientListFilter";
import { useState } from "react";
import { useClients } from "../../../lib/hooks/useClients";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import { DEFAULT_FILTERS_Clients } from "../../../lib/api/Clients/DefaultFiltersClients";
import { FilterDescriptor } from "../../../lib/types/Common/FilterDescriptor";

export default function ClientsDashboard() {
  const [filters, setFilters] = useState<FilterDescriptor[]>(
    DEFAULT_FILTERS_Clients
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 10;
  const { clients, pagination, isLoadingClients } = useClients(
    { pageNumber, pageSize },
    filters
  );

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1); // сбросить страницу
  };

  return (
    <>
      {clients?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <EmptyPage input={"Клиенты не найдены"} />
          </Grid2>
          <Grid2 size={3}>
            <ClientListFilter onApplyFilters={handleApplyFilters} />
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
            />
          </Grid2>
          <Grid2 size={3}>
            <ClientListFilter onApplyFilters={handleApplyFilters} />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
