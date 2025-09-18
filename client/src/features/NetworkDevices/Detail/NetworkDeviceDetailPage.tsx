import { Link, useParams } from "react-router";
import { useNetworkDevices } from "../../../lib/hooks/useNetworkDevices";
import { DEFAULT_PAGINATION_PARAMS } from "../../../lib/types/Common/DEFAULT_PAGINATION_PARAMS";
import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Typography,
  CircularProgress,
  Alert,
  Chip,
} from "@mui/material";
import NetworkDevicePort from "./NetworkDevicePort";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import DeviceHubIcon from "@mui/icons-material/DeviceHub";

export default function NetworkDeviceDetailPage() {
  const { id } = useParams();
  const { networkDevice, isLoadingNetworkDevice, error } = useNetworkDevices(
    DEFAULT_PAGINATION_PARAMS,
    [],
    { property: "", descending: false },
    id
  );

  if (isLoadingNetworkDevice) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="60vh"
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Loading device details...
        </Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box>
        <Button
          variant="outlined"
          color="primary"
          startIcon={<ArrowBackIcon />}
          component={Link}
          to={`/networkDevices`}
          sx={{ mb: 2 }}
        >
          Back to Devices
        </Button>
        <Alert severity="error" sx={{ borderRadius: 2 }}>
          Error loading device: {error.message}
        </Alert>
      </Box>
    );
  }

  if (!networkDevice) {
    return (
      <Box>
        <Button
          variant="outlined"
          color="primary"
          startIcon={<ArrowBackIcon />}
          component={Link}
          to={`/networkDevices`}
          sx={{ mb: 2 }}
        >
          Back to Devices
        </Button>
        <Alert severity="warning" sx={{ borderRadius: 2 }}>
          Device not found
        </Alert>
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" alignItems="center" mt={2} mb={3}>
        <Button
          variant="outlined"
          color="primary"
          startIcon={<ArrowBackIcon />}
          component={Link}
          to={`/networkDevices`}
          sx={{ mr: 2 }}
        >
          Back to Devices
        </Button>
        <DeviceHubIcon sx={{ mr: 1, color: "primary.main" }} />
        <Typography variant="h5" component="h1" fontWeight="bold">
          Network Device Details
        </Typography>
      </Box>

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
    </Box>
  );
}
