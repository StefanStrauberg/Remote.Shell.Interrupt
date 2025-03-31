import {
  Avatar,
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { Checklist, LocalLibrary, Badge } from "@mui/icons-material";
import { Gate } from "../../lib/types/Gate";
import { Link } from "react-router";
import { useGates } from "../../lib/hooks/useGates";

type Props = {
  gate: Gate;
};

export default function GateCard({ gate }: Props) {
  const { deleteGate } = useGates(); // Use the deleteGate mutation

  const handleDelete = () => {
    if (window.confirm(`Are you sure you want to delete "${gate.name}"?`)) {
      deleteGate.mutate(gate.id!, {
        onSuccess: () => {
          console.log(`Gate with ID ${gate.id} deleted successfully`);
        },
        onError: (error) => {
          console.error("Failed to delete gate:", error);
        },
      });
    }
  };

  const processAltText = (typeOfNetworkDevice: string) => {
    switch (typeOfNetworkDevice) {
      case "Huawei":
        return "Huawei networks logo";
      case "Juniper":
        return "Juniper networks logo";
      case "Extreme":
        return "Extreme networks logo";
      default:
        return "Undefined type of network device";
    }
  };

  const processSrcImg = (typeOfNetworkDevice: string) => {
    switch (typeOfNetworkDevice) {
      case "Huawei":
        return "images/Huawei_Logo.png";
      case "Juniper":
        return "images/Juniper_Networks_Logo.png";
      case "Extreme":
        return "images/Extreme_Networks_Logo.png";
      default:
        return "";
    }
  };

  return (
    <Card elevation={5} sx={{ borderRadius: 4, fontSize: 18 }}>
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
              alt={processAltText(gate.typeOfNetworkDevice)}
              src={processSrcImg(gate.typeOfNetworkDevice)}
              style={{
                height: "100%",
                width: "100%", // Maintain aspect ratio
              }}
            />
          </Avatar>
        }
        title={
          <Typography sx={{ fontWeight: "bold" }}>
            IP Address: {gate.ipAddress}
          </Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Badge sx={{ mr: 1 }} />
          <Typography variant="body2">Name: {gate.name}</Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <LocalLibrary sx={{ mr: 1 }} />
          <Typography variant="body2">Community: {gate.community}</Typography>
        </Box>
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <Checklist sx={{ mr: 1 }} />
            <Typography variant="body2">
              Type: {gate.typeOfNetworkDevice}
            </Typography>
          </Box>
          <ButtonGroup variant="contained" aria-label="Basic button group">
            <Button color="info" component={Link} to={`/gates/${gate.id}`}>
              View
            </Button>
            <Button
              color="primary"
              component={Link}
              to={`/editGate/${gate.id}`}
            >
              Edit
            </Button>
            <Button color="error" onClick={handleDelete}>
              delete
            </Button>
          </ButtonGroup>
        </Box>
      </CardContent>
    </Card>
  );
}
