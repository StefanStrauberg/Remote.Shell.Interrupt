import {
  Box,
  Card,
  CardContent,
  CardHeader,
  Chip,
  Typography,
} from "@mui/material";
import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import NetworkDevicePort from "../../NetworkDevices/Detail/NetworkDevicePort";

type Props = {
  networkDevice: NetworkDevice;
};

export default function RouterBox({ networkDevice }: Props) {
  return (
    <Card
      elevation={5}
      sx={{
        mt: 2,
        borderRadius: 4,
        boxShadow: 3,
        fontSize: 18,
      }}
    >
      <Card elevation={2} sx={{ borderRadius: 3, overflow: "hidden" }}>
        <CardHeader
          title={
            <Box display="flex" alignItems="center">
              <Typography variant="h6" fontWeight="bold" sx={{ mr: 1 }}>
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
          sx={{ backgroundColor: "primary.light", color: "white", py: 2 }}
        />
        <CardContent sx={{ p: 0 }}>
          {networkDevice.portsOfNetworkDevice.length > 0 ? (
            networkDevice.portsOfNetworkDevice.map((port) => (
              <NetworkDevicePort key={port.id} port={port} />
            ))
          ) : (
            <Box sx={{ p: 3, textAlign: "center" }}>
              <Typography variant="body1" color="text.secondary">
                No ports available
              </Typography>
            </Box>
          )}
        </CardContent>
      </Card>
    </Card>
  );
}
