import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { useGates } from "../../lib/hooks/useGates";
import { Link, useParams } from "react-router";
import {
  Checklist,
  Fingerprint,
  LocalLibrary,
  Place,
  Storage,
} from "@mui/icons-material";

export default function GateDetailPage() {
  const { id } = useParams();
  const { gate, isLoadingGate } = useGates(0, 0, id);

  if (isLoadingGate) return <Typography>Loading...</Typography>;

  if (!gate) return <Typography>Activity not found</Typography>;

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
          <Fingerprint sx={{ mr: 1 }} />
          <Typography variant="body1" sx={{ color: "gray" }}>
            ID: {gate.id}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Place sx={{ mr: 1 }} />
          <Typography variant="body1">IP Address: {gate.ipAddress}</Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <LocalLibrary sx={{ mr: 1 }} />
          <Typography variant="body1">Community: {gate.community}</Typography>
        </Box>
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <Checklist sx={{ mr: 1 }} />
            <Typography variant="body1">
              Type: {gate.typeOfNetworkDevice}
            </Typography>
          </Box>
          <Button
            variant="contained"
            color="info"
            component={Link}
            to={`/gates`}
          >
            Back
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
