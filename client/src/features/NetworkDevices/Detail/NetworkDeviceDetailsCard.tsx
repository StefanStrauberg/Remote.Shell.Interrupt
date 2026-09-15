import {
  Box,
  Card,
  CardContent,
  CardHeader,
  Chip,
  Typography,
} from "@mui/material";
import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import NetworkDevicePort from "./NetworkDevicePort";

type Props = { networkDevice: NetworkDevice };

export default function NetworkDeviceDetailsCard({ networkDevice }: Props) {
  return (
    <Card sx={{ overflow: "hidden" }}>
      <CardHeader
        title={
          <Box display="flex" alignItems="center" gap={1} flexWrap="wrap">
            <Typography variant="h6">
              {networkDevice.networkDeviceName}
            </Typography>
            <Chip
              label={networkDevice.host}
              size="small"
              color="primary"
              variant="outlined"
            />
          </Box>
        }
      />
      <CardContent sx={{ p: 0 }}>
        {networkDevice.portsOfNetworkDevice.length > 0 ? (
          networkDevice.portsOfNetworkDevice.map((port) => (
            <NetworkDevicePort key={port.id} port={port} />
          ))
        ) : (
          <Box sx={{ p: 4, textAlign: "center" }}>
            <Typography color="text.secondary">No ports available</Typography>
          </Box>
        )}
      </CardContent>
    </Card>
  );
}
