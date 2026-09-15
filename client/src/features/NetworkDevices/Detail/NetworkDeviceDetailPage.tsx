import { Link, useParams } from "react-router";
import { useNetworkDeviceQuery } from "../api/networkDevicesQueries";
import { Box, Button } from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import DeviceHubIcon from "@mui/icons-material/DeviceHub";
import { routes } from "../../../app/router/paths";
import PageHeader from "../../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../../app/shared/components/PageFeedback";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import NetworkDeviceDetailsCard from "./NetworkDeviceDetailsCard";

export default function NetworkDeviceDetailPage() {
  const { id } = useParams();
  const networkDeviceQuery = useNetworkDeviceQuery(id);
  const networkDevice = networkDeviceQuery.data;

  if (networkDeviceQuery.isLoading) {
    return <PageLoading message="Loading device details…" />;
  }

  if (networkDeviceQuery.isError) {
    return (
      <Box>
        <Button
          variant="outlined"
          color="primary"
          startIcon={<ArrowBackIcon />}
          component={Link}
          to={routes.networkDevices}
          sx={{ mb: 2 }}
        >
          Back to Devices
        </Button>
        <PageError
          title="Unable to load network device"
          error={networkDeviceQuery.error}
        />
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
          to={routes.networkDevices}
          sx={{ mb: 2 }}
        >
          Back to Devices
        </Button>
        <EmptyPage input="Network device not found" />
      </Box>
    );
  }

  return (
    <Box>
      <PageHeader
        title="Network device details"
        description="Ports, subports, VLANs, and learned MAC addresses"
        icon={DeviceHubIcon}
        action={
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            component={Link}
            to={routes.networkDevices}
          >
            Back to devices
          </Button>
        }
      />
      <NetworkDeviceDetailsCard networkDevice={networkDevice} />
    </Box>
  );
}
