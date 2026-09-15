import { Typography, Box } from "@mui/material";
import { CompoundObject } from "../../lib/types/NetworkDevices/CompoundObject";
import ClientBox from "./ClientPart/ClientBox";
import RouterBox from "./RouterPart/RouterBox";
import EmptyPage from "../../app/shared/components/EmptyPage";

type Props = {
  data: CompoundObject | undefined;
};

export default function MainPageList({ data }: Props) {
  if (!data) {
    return (
      <EmptyPage
        input="No search data"
        description="Enter a VLAN ID to start a network search."
      />
    );
  }

  const hasClients = data.clients.length > 0;
  const hasNetworkDevices = data.networkDevices.length > 0;

  if (!hasClients && !hasNetworkDevices) {
    return (
      <EmptyPage
        input="No devices found"
        description="No clients or network devices are assigned to the specified VLAN."
      />
    );
  }

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* Clients Section */}
      {hasClients && (
        <Box>
          <Typography variant="h6" gutterBottom color="primary">
            Clients ({data.clients.length})
          </Typography>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            {data.clients.map((client) => (
              <ClientBox key={client.id} client={client} />
            ))}
          </Box>
        </Box>
      )}

      {/* Network Devices Section */}
      {hasNetworkDevices && (
        <Box>
          <Typography variant="h6" gutterBottom color="primary">
            Network Devices ({data.networkDevices.length})
          </Typography>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            {data.networkDevices.map((networkDevice) => (
              <RouterBox key={networkDevice.id} networkDevice={networkDevice} />
            ))}
          </Box>
        </Box>
      )}
    </Box>
  );
}
