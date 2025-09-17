import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
  Chip,
  Stack,
  Tooltip,
} from "@mui/material";
import {
  AlternateEmail,
  CheckBox,
  ContactPage,
  ContactPhone,
  Fingerprint,
  Gavel,
  Visibility,
} from "@mui/icons-material";
import { Link } from "react-router";
import { ClientShort } from "../../../lib/types/Clients/ClientShort";

type Props = {
  client: ClientShort;
  viewMode?: "grid" | "list";
};

export default function ClientCard({ client, viewMode = "grid" }: Props) {
  return (
    <Card
      variant="outlined"
      sx={{
        borderRadius: 2,
        height: viewMode === "list" ? "auto" : "100%",
        display: "flex",
        flexDirection: "column",
        transition: "all 0.2s ease-in-out",
        "&:hover": {
          boxShadow: 3,
          transform: "translateY(-2px)",
        },
        opacity: client.working ? 1 : 0.7,
      }}
    >
      <CardHeader
        title={
          <Tooltip title={client.name} arrow placement="top">
            <Typography
              variant="h6"
              component="h3"
              noWrap
              sx={{
                overflow: "hidden",
                textOverflow: "ellipsis",
                maxWidth: "100%",
              }}
            >
              {client.name}
            </Typography>
          </Tooltip>
        }
        subheader={
          <Stack direction="row" spacing={1} mt={1} flexWrap="wrap">
            <Chip
              icon={<CheckBox />}
              label={client.working ? "Active" : "Inactive"}
              size="small"
              color={client.working ? "success" : "default"}
              variant="outlined"
            />
            <Chip
              icon={<CheckBox />}
              label={client.antiDDOS ? "AntiDDOS" : "No AntiDDOS"}
              size="small"
              color={client.antiDDOS ? "primary" : "default"}
              variant="outlined"
            />
          </Stack>
        }
        sx={{
          pb: 1,
          // Prevent header from growing too much
          "& .MuiCardHeader-content": {
            minWidth: 0, // Allow text overflow
            overflow: "hidden",
          },
        }}
      />

      <Divider />

      <CardContent sx={{ flexGrow: 1, p: 2 }}>
        <Stack spacing={1.5}>
          <Box display="flex" alignItems="center">
            <Fingerprint
              sx={{
                mr: 1,
                color: "text.secondary",
                fontSize: 20,
                flexShrink: 0,
              }}
            />
            <Typography variant="body2" noWrap>
              <strong>ID:</strong> {client.idClient}
            </Typography>
          </Box>

          <Box display="flex" alignItems="center">
            <Gavel
              sx={{
                mr: 1,
                color: "text.secondary",
                fontSize: 20,
                flexShrink: 0,
              }}
            />
            <Typography variant="body2" noWrap>
              <strong>Contract:</strong> {client.nrDogovor}
            </Typography>
          </Box>

          <Box display="flex" alignItems="center">
            <ContactPage
              sx={{
                mr: 1,
                color: "text.secondary",
                fontSize: 20,
                flexShrink: 0,
              }}
            />
            <Tooltip title={client.contactT} arrow>
              <Typography variant="body2" noWrap>
                <strong>Tech Contact:</strong> {client.contactT}
              </Typography>
            </Tooltip>
          </Box>

          <Box display="flex" alignItems="center">
            <ContactPhone
              sx={{
                mr: 1,
                color: "text.secondary",
                fontSize: 20,
                flexShrink: 0,
              }}
            />
            <Typography variant="body2" noWrap>
              <strong>Tech Phone:</strong> {client.telephoneT}
            </Typography>
          </Box>

          <Box display="flex" alignItems="center">
            <AlternateEmail
              sx={{
                mr: 1,
                color: "text.secondary",
                fontSize: 20,
                flexShrink: 0,
              }}
            />
            <Tooltip title={client.emailT} arrow>
              <Typography variant="body2" noWrap>
                <strong>Tech Email:</strong> {client.emailT}
              </Typography>
            </Tooltip>
          </Box>
        </Stack>
      </CardContent>

      <Divider />

      <CardContent sx={{ p: 2 }}>
        <Button
          variant="outlined"
          fullWidth
          startIcon={<Visibility />}
          component={Link}
          to={`/clients/${client.id}`}
          size="small"
        >
          View Details
        </Button>
      </CardContent>
    </Card>
  );
}
