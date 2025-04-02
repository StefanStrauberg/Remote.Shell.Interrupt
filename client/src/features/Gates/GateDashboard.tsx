import { Grid2 } from "@mui/material";
import GateListPage from "./GateListPage";
import GateFilter from "./GateFilter";

export default function GateDashboard() {
  return (
    <Grid2 container spacing={3}>
      <Grid2 size={9}>
        <GateListPage />
      </Grid2>
      <Grid2 size={3}>
        <GateFilter />
      </Grid2>
    </Grid2>
  );
}
