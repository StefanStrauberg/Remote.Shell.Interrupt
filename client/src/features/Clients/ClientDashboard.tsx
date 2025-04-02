import { Grid2 } from "@mui/material";
import ClientListPage from "./ClientListPage";
import ClientFilter from "./ClientFilter";

export default function ClientDashboard() {
  return (
    <Grid2 container spacing={3}>
      <Grid2 size={9}>
        <ClientListPage />
      </Grid2>
      <Grid2 size={3}>
        <ClientFilter />
      </Grid2>
    </Grid2>
  );
}
