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
import { useDeleteGateMutation } from "../api/gatesQueries";
import { toast } from "react-toastify";
import { routes } from "../../../app/router/paths";
import { getNetworkDeviceBrand } from "../../../lib/presentation/networkDeviceBrand";

type GateCardProps = {
  gate: Gate;
};

export default function GateCard({ gate }: GateCardProps) {
  const deleteGate = useDeleteGateMutation();
  const brand = getNetworkDeviceBrand(gate.typeOfNetworkDevice);

  const handleDelete = () => {
    if (window.confirm(`Are you sure you want to delete "${gate.name}"?`)) {
      deleteGate.mutate(gate.id, {
        onSuccess: () => {
          toast.success(`Gate "${gate.name}" was deleted successfully.`);
        },
        onError: (error) => {
          toast.error(`Failed to delete gate: ${error.message}`);
        },
      });
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
              to={routes.gate(gate.id)}
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
