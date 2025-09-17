import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
  Chip,
  Tooltip,
} from "@mui/material";
import { Checklist, LocalLibrary, LocationOn } from "@mui/icons-material";
import { Gate } from "../../../lib/types/Gates/Gate";
import { Link } from "react-router";
import { useGates } from "../../../lib/hooks/useGates";
import { DEFAULT_PAGINATION_PARAMS } from "../../../lib/types/Common/DEFAULT_PAGINATION_PARAMS";

type GateCardProps = {
  gate: Gate;
};

export default function GateCard({ gate }: GateCardProps) {
  const { deleteGate } = useGates(DEFAULT_PAGINATION_PARAMS, [], {
    property: "",
    descending: false,
  });

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
        return "Network device logo";
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
              alt={processAltText(gate.typeOfNetworkDevice)}
              src={processSrcImg(gate.typeOfNetworkDevice)}
              style={{
                height: "100%",
                width: "100%",
                objectFit: "contain",
              }}
            />
          </Avatar>
        }
        title={
          <Tooltip title={gate.name} arrow>
            <Typography
              variant="h6"
              component="h3"
              noWrap
              sx={{ fontWeight: "bold" }}
            >
              {gate.name}
            </Typography>
          </Tooltip>
        }
        subheader={
          <Chip
            label={gate.typeOfNetworkDevice}
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
            <strong>IP:</strong> {gate.ipAddress}
          </Typography>
        </Box>

        <Box display="flex" alignItems="center">
          <LocalLibrary
            sx={{ mr: 1, color: "text.secondary", flexShrink: 0 }}
          />
          <Typography variant="body2" noWrap>
            <strong>Community:</strong> {gate.community}
          </Typography>
        </Box>
      </CardContent>

      <Divider />

      <CardContent sx={{ p: 2 }}>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Box display="flex" alignItems="center">
            <Checklist sx={{ mr: 1, color: "text.secondary" }} />
            <Typography variant="body2">
              Type: {gate.typeOfNetworkDevice}
            </Typography>
          </Box>

          <Box display="flex" gap={1}>
            <Button
              variant="outlined"
              component={Link}
              to={`/gates/${gate.id}`}
              size="small"
            >
              Edit
            </Button>

            <Button
              variant="outlined"
              color="error"
              onClick={handleDelete}
              size="small"
              disabled={deleteGate.isPending}
            >
              {deleteGate.isPending ? "Deleting..." : "Delete"}
            </Button>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}
