import { Grid2 } from "@mui/material";
import NetworkDeviceList from "./NetworkDeviceList";
import { NetworkDevice } from "../../../lib/types/NetworkDevice";

type Props = {
  networkDevices: NetworkDevice[];
};

export default function NetworkDeviceDashboard({ networkDevices }: Props) {
  return (
    <Grid2 container>
      <Grid2>
        <NetworkDeviceList networkDevices={networkDevices} />
      </Grid2>
    </Grid2>
  );
}
