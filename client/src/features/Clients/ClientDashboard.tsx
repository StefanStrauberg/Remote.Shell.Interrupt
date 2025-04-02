import { Grid2 } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientFilter from "./ClientFilter";
import { useClients } from "../../lib/hooks/useClients";

export default function ClientDashboard() {
  // Hook for fetching data
  const { clients = [] } = useClients();

  return (
    <>
      {clients?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={12}>
            <ClientListPage />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <ClientListPage />
          </Grid2>
          <Grid2 size={3}>
            <ClientFilter />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
