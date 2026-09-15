import { Box } from "@mui/material";
import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import NetworkDeviceDetailsCard from "../../NetworkDevices/Detail/NetworkDeviceDetailsCard";

type Props = {
  networkDevice: NetworkDevice;
};

export default function RouterBox({ networkDevice }: Props) {
  return (
    <Box mt={2}>
      <NetworkDeviceDetailsCard networkDevice={networkDevice} />
    </Box>
  );
}
