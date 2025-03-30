import { Grid2 } from "@mui/material";
import NetworkDeviceList from "./NetworkDeviceList";

export default function NetworkDeviceDashboard() {
  return (
    <Grid2 container>
      <Grid2>
        <NetworkDeviceList />
      </Grid2>
    </Grid2>
  );
}
