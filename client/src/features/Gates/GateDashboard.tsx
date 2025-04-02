import { Grid2 } from "@mui/material";
import GateListPage from "./GateListPage";
import GateFilter from "./GateFilter";
import { useGates } from "../../lib/hooks/useGates";

export default function GateDashboard() {
  // Hook for fetching data
  const { gates = [] } = useGates();

  return (
    <>
      {gates?.length === 0 ? (
        <Grid2 container spacing={3}>
          <Grid2 size={12}>
            <GateListPage />
          </Grid2>
        </Grid2>
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={9}>
            <GateListPage />
          </Grid2>
          <Grid2 size={3}>
            <GateFilter />
          </Grid2>
        </Grid2>
      )}
    </>
  );
}
