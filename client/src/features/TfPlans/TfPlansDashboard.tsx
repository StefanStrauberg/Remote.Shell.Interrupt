import { Grid2 } from "@mui/material";
import TfPlanFilter from "./TfPlanFilter";
import TfPlanListPage from "./TfPlanListPage";

export default function TfPlansDashboard() {
  return (
    <Grid2 container spacing={3}>
      <Grid2 size={9}>
        <TfPlanListPage />
      </Grid2>
      <Grid2 size={3}>
        <TfPlanFilter />
      </Grid2>
    </Grid2>
  );
}
