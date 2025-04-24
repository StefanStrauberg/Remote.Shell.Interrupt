import { Card, CardHeader, Divider, Typography } from "@mui/material";
import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import RouterPort from "./RouterPort";

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
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>
            {networkDevice.networkDeviceName} - {networkDevice.host}
          </Typography>
        }
      />
      <Divider />
      {networkDevice.portsOfNetworkDevice.length > 0 ? (
        networkDevice.portsOfNetworkDevice.map((port) => (
          <RouterPort key={port.id} port={port} />
        ))
      ) : (
        <Typography sx={{ px: 2, py: 1 }}>Нет доступных портов</Typography>
      )}
    </Card>
  );
}
