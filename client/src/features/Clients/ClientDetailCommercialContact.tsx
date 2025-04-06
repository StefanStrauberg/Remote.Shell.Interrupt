import { Box, Typography } from "@mui/material";
import { Client } from "../../lib/types/Client";
import { AlternateEmail, ContactPage, LocalPhone } from "@mui/icons-material";

type Props = {
  client: Client;
};

export default function ClientDetailCommercialContact({ client }: Props) {
  return (
    <>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <ContactPage
          sx={{ mr: 1, color: client.contactC ? "inherit" : "gray" }}
        />
        <Typography
          variant="body1"
          sx={{
            color: client.contactC ? "inherit" : "gray",
          }}
        >
          Контакт: {client.contactC || "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <LocalPhone
          sx={{ mr: 1, color: client.telephoneC ? "inherit" : "gray" }}
        />
        <Typography
          variant="body1"
          sx={{
            color: client.telephoneC ? "inherit" : "gray",
          }}
        >
          Телефон: {client.telephoneC || "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <AlternateEmail
          sx={{ mr: 1, color: client.emailC ? "inherit" : "gray" }}
        />
        <Typography
          variant="body1"
          sx={{
            color: client.emailC ? "inherit" : "gray",
          }}
        >
          Email: {client.emailC || "Нет информации"}
        </Typography>
      </Box>
    </>
  );
}
