import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { NetworkDevice } from "../../lib/types/NetworkDevices/NetworkDevice";
import { Checklist, LocalLibrary, LocationOn } from "@mui/icons-material";

type Props = {
  networkDevice: NetworkDevice;
};

export default function NetworkDeviceCard({ networkDevice }: Props) {
  const processAltText = (typeOfNetworkDevice: string) => {
    switch (typeOfNetworkDevice) {
      case "Huawei":
        return "Huawei logo";
      case "Juniper":
        return "Juniper logo";
      case "Extreme":
        return "Extreme logo";
      case "Cisco":
        return "Cisco logo";
      case "FortiGate":
        return "Fortinet logo";
      default:
        return "Undefined type of network device";
    }
  };

  const processSrcImg = (typeOfNetworkDevice: string) => {
    switch (typeOfNetworkDevice) {
      case "Huawei":
        return "images/Huawei_Logo.png";
      case "Juniper":
        return "images/Juniper_Logo.png";
      case "Extreme":
        return "images/Extreme_Logo.png";
      case "Cisco":
        return "images/Cisco_Logo.png";
      case "FortiGate":
        return "images/Fortinet_Logo.png";
      default:
        return "";
    }
  };

  return (
    <Card
      variant="outlined"
      elevation={5}
      sx={{
        borderRadius: 4,
        boxShadow: 3,
        fontSize: 18,
      }}
    >
      <CardHeader
        avatar={
          <Avatar
            sx={{
              height: 100,
              width: 100,
              overflow: "hidden",
              position: "relative",
              bgcolor: "transparent",
            }}
          >
            <img
              alt={processAltText(networkDevice.typeOfNetworkDevice)}
              src={processSrcImg(networkDevice.typeOfNetworkDevice)}
              style={{
                height: "100%",
                width: "100%", // Maintain aspect ratio
              }}
            />
          </Avatar>
        }
        title={
          <Typography variant="body1" sx={{ fontWeight: "bold" }}>
            {networkDevice.networkDeviceName}
          </Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <LocationOn sx={{ mr: 1 }} />
          <Typography variant="body1">
            IP Address: {networkDevice.host}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <LocalLibrary sx={{ mr: 1 }} />
          <Typography variant="body1">
            General Inforamtion: {networkDevice.generalInformation}
          </Typography>
        </Box>
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <Checklist sx={{ mr: 1 }} />
            <Typography variant="body1">
              Type: {networkDevice.typeOfNetworkDevice}
            </Typography>
          </Box>
          <Button variant="contained" color="error">
            Удалить
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
