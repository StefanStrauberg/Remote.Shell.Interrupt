import {
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { NetworkDevice } from "../../lib/types/NetworkDevice";

type Props = {
  networkDevice: NetworkDevice;
};

export default function NetworkDeviceCard({ networkDevice }: Props) {
  return (
    <Box sx={{ paddingTop: 2 }}>
      <Paper elevation={3} sx={{ padding: 2 }}>
        <Typography variant="h5">General Information</Typography>
        <Typography variant="body1">IP: {networkDevice.host}</Typography>
        <Typography variant="body1">
          Type: {networkDevice.typeOfNetworkDevice}
        </Typography>
        <Typography variant="body1">
          Name: {networkDevice.networkDeviceName}
        </Typography>
        <Typography variant="body1">
          Information: {networkDevice.generalInformation}
        </Typography>
        <Typography variant="h6" sx={{ marginTop: 2 }}>
          Ports
        </Typography>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Interface Number</TableCell>
                <TableCell>Interface Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Speed</TableCell>
                <TableCell>MAC Address</TableCell>
                <TableCell>VLANs</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {networkDevice.portsOfNetworkDevice.map((port) => (
                <TableRow key={port.id}>
                  <TableCell>{port.interfaceNumber}</TableCell>
                  <TableCell>{port.interfaceName}</TableCell>
                  <TableCell>{port.interfaceType}</TableCell>
                  <TableCell>{port.interfaceStatus}</TableCell>
                  <TableCell>{port.interfaceSpeed}</TableCell>
                  <TableCell>{port.macAddress}</TableCell>
                  <TableCell>
                    {port.vlaNs.map((vlan) => (
                      <div key={vlan.vlanTag}>
                        {vlan.vlanTag}: {vlan.vlanName}
                      </div>
                    ))}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Box>
  );
}
