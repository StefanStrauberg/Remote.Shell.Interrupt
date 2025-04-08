import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import {
  AlternateEmail,
  CheckBox,
  ContactPage,
  ContactPhone,
  Fingerprint,
  Gavel,
} from "@mui/icons-material";
import { Link } from "react-router";
import { ClientShort } from "../../../lib/types/ClientShort";

type Props = {
  client: ClientShort;
};

export default function ClientCard({ client }: Props) {
  return (
    <Card
      variant="outlined"
      elevation={5}
      sx={
        client.working
          ? {
              borderRadius: 4,
              boxShadow: 3,
              fontSize: 18,
            }
          : {
              borderRadius: 4,
              boxShadow: 3,
              fontSize: 18,
              color: "gray",
            }
      }
    >
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>{client.name}</Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Fingerprint sx={{ mr: 1 }} />
          <Typography variant="body1">ID Клиента: {client.idClient}</Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Gavel sx={{ mr: 1 }} />
          <Typography variant="body1">
            Номер договора: {client.nrDogovor}
          </Typography>
        </Box>
        <Divider />
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
              color: client.working ? "green" : "gray",
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
              sx={
                client.working
                  ? {
                      mr: 1,
                      color: client.antiDDOS ? "green" : "red",
                    }
                  : {
                      mr: 1,
                      color: client.antiDDOS ? "green" : "gray",
                    }
              }
            />
            <Typography variant="body1">
              AntiDDOS: {client.antiDDOS ? "Да" : "Нет"}
            </Typography>
          </Box>
          <Button
            variant="contained"
            color={client.working ? "primary" : "inherit"}
            component={Link}
            to={`/clients/${client.id}`}
          >
            Обзор
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
