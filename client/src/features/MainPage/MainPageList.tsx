import { Typography, Box, Paper } from "@mui/material";
import { CompoundObject } from "../../lib/types/NetworkDevices/CompoundObject";
import ClientBox from "./ClientPart/ClientBox";
import RouterBox from "./RouterPart/RouterBox";
import SearchOffIcon from "@mui/icons-material/SearchOff";

type Props = {
  data: CompoundObject | undefined;
};

export default function MainPageList({ data }: Props) {
  if (!data) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="200px"
      >
        <Typography color="text.secondary">
          No data available. Please search for a VLAN ID.
        </Typography>
      </Box>
    );
  }

  const hasClients = data.clients.length > 0;
  const hasNetworkDevices = data.networkDevices.length > 0;

  if (!hasClients && !hasNetworkDevices) {
    return (
      <Paper sx={{ p: 4, textAlign: "center" }}>
        <SearchOffIcon sx={{ fontSize: 64, color: "text.secondary", mb: 2 }} />
        <Typography variant="h6" color="text.secondary" gutterBottom>
          No devices found
        </Typography>
        <Typography variant="body2" color="text.secondary">
          No clients or network devices found for the specified VLAN.
        </Typography>
      </Paper>
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
