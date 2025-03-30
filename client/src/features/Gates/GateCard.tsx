import {
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { Checklist, LocalLibrary, Place, Storage } from "@mui/icons-material";
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

  return (
    <Card elevation={5} sx={{ borderRadius: 4, fontSize: 18 }}>
      <CardHeader
        avatar={<Storage />}
        title={
          <Typography sx={{ fontWeight: "bold" }}>Name: {gate.name}</Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Place sx={{ mr: 1 }} />
          <Typography variant="body2">IP Address: {gate.ipAddress}</Typography>
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
