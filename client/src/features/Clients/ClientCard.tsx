import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { ClientShort } from "../../lib/types/ClientShort";
import {
  AlternateEmail,
  CheckBox,
  ContactPage,
  ContactPhone,
} from "@mui/icons-material";
import { Link } from "react-router";

type Props = {
  client: ClientShort;
};

export default function ClientCard({ client }: Props) {
  return (
    <Card elevation={5} sx={{ borderRadius: 4, boxShadow: 3, fontSize: 18 }}>
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>{client.name}</Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <ContactPage sx={{ mr: 1 }} />
          <Typography variant="body1">
            Контакт технический: {client.contactT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <ContactPhone sx={{ mr: 1 }} />
          <Typography variant="body1">
            Телефон технический: {client.telephoneT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <AlternateEmail sx={{ mr: 1 }} />
          <Typography variant="body1">
            Email технический: {client.emailT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <CheckBox
            sx={{
              mr: 1,
              color: client.working ? "green" : "red",
            }}
          />
          <Typography variant="body1">
            Работает: {client.working ? "Да" : "Нет"}
          </Typography>
        </Box>
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <CheckBox
              sx={{ mr: 1, color: client.antiDDOS ? "green" : "red" }}
            />
            <Typography variant="body1">
              AntiDDOS: {client.antiDDOS ? "Да" : "Нет"}
            </Typography>
          </Box>
          <Button
            variant="contained"
            component={Link}
            to={`/clients/${client.id}`}
          >
            View
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
