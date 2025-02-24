import { Box } from "@mui/material";
import { NetworkDevice } from "../../../lib/types/NetworkDevice";
import NetworkDeviceCard from "./NetworkDeviceCard";

type Props = {
  networkDevices: NetworkDevice[];
};

export default function NetworkDeviceList({ networkDevices }: Props) {
  return (
    <Box>
      {networkDevices.map((networkDevice) => (
        <NetworkDeviceCard
          key={networkDevice.id}
          networkDevice={networkDevice}
        />
      ))}
    </Box>
  );
}
