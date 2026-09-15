import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
  Tooltip,
  Chip,
} from "@mui/material";
import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import { Checklist, LocalLibrary, LocationOn } from "@mui/icons-material";
import { Link } from "react-router";
import { routes } from "../../../app/router/paths";
import { getNetworkDeviceBrand } from "../../../lib/presentation/networkDeviceBrand";

type Props = {
  networkDevice: NetworkDevice;
};

export default function NetworkDeviceCard({ networkDevice }: Props) {
  const brand = getNetworkDeviceBrand(networkDevice.typeOfNetworkDevice);

  return (
    <Card
      variant="outlined"
      sx={{
        borderRadius: 2,
        height: "100%",
        display: "flex",
        flexDirection: "column",
        transition: "all 0.2s ease-in-out",
        "&:hover": {
          boxShadow: 3,
          transform: "translateY(-2px)",
        },
      }}
    >
      <CardHeader
        avatar={
          <Avatar
            sx={{
              height: 60,
              width: 60,
              bgcolor: "transparent",
            }}
            variant="square"
          >
            <img
              alt={brand.alt}
              src={brand.imageSrc}
              style={{
                height: "100%",
                width: "100%",
                objectFit: "contain",
              }}
            />
          </Avatar>
        }
        title={
          <Tooltip title={networkDevice.networkDeviceName} arrow>
            <Typography
              variant="h6"
              component="h3"
              noWrap
              sx={{ fontWeight: "bold" }}
            >
              {networkDevice.networkDeviceName}
            </Typography>
          </Tooltip>
        }
        subheader={
          <Chip
            label={networkDevice.typeOfNetworkDevice}
            size="small"
            color="primary"
            variant="outlined"
          />
        }
      />

      <Divider />

      <CardContent sx={{ flexGrow: 1, p: 2 }}>
        <Box display="flex" alignItems="center" mb={2}>
          <LocationOn sx={{ mr: 1, color: "text.secondary", flexShrink: 0 }} />
          <Typography variant="body2" noWrap>
            <strong>IP:</strong> {networkDevice.host}
          </Typography>
        </Box>

        <Box display="flex" alignItems="flex-start">
          <LocalLibrary
            sx={{ mr: 1, color: "text.secondary", mt: 0.25, flexShrink: 0 }}
          />
          <Typography variant="body2" sx={{ wordBreak: "break-word" }}>
            <strong>Info:</strong>{" "}
            {networkDevice.generalInformation || "No information"}
          </Typography>
        </Box>
      </CardContent>

      <Divider />

      <CardContent sx={{ p: 2 }}>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Box display="flex" alignItems="center">
            <Checklist sx={{ mr: 1, color: "text.secondary" }} />
            <Typography variant="body2">
              Type: {networkDevice.typeOfNetworkDevice}
            </Typography>
          </Box>

          <Button
            variant="outlined"
            component={Link}
            to={routes.networkDevice(networkDevice.id)}
            size="small"
          >
            View
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
