import { Grid2 } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientListFilter from "./ClientListFilter";
import { useClients } from "../../lib/hooks/useClients";
import { useState } from "react";
import ClientsEmptyPage from "./ClientsEmptyPage";
import { ClientFilter } from "../../lib/types/ClientFilter";

export default function ClientDashboard() {
  const [filters, setFilters] = useState<ClientFilter>({
    Working: { op: "==", value: true },
  });
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 10;

  const { clients, pagination, isPending } = useClients(
    pageNumber,
    pageSize,
    filters
  );

  const handleApplyFilters = (newFilters: ClientFilter) => {
    setFilters(newFilters);
    setPageNumber(1); // сбросить страницу
  };

  return (
    <>
      {clients?.length === 0 ? (
        <ClientsEmptyPage onApplyFilters={handleApplyFilters} />
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <ClientListPage
              clients={clients}
              isPending={isPending}
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
