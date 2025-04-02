import { Grid2 } from "@mui/material";
import TfPlanFilter from "./TfPlanFilter";
import TfPlanListPage from "./TfPlanListPage";
import { useTfPlans } from "../../lib/hooks/useTfPlans";

export default function TfPlansDashboard() {
  // Hook for fetching data
  const { tfPlans = [] } = useTfPlans();

  return (
    <>
      {tfPlans?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={12}>
            <TfPlanListPage />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <TfPlanListPage />
          </Grid2>
          <Grid2 size={3}>
            <TfPlanFilter />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
