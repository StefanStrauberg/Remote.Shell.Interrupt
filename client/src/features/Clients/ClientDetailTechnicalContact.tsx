import { Box, Typography } from "@mui/material";
import { Client } from "../../lib/types/Client";
import { AlternateEmail, ContactPage, LocalPhone } from "@mui/icons-material";

type Props = {
  client: Client;
};

export default function ClientDetailTechnicalContact({ client }: Props) {
  return (
    <>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <ContactPage
          sx={{ mr: 1, color: client.contactT ? "inherit" : "gray" }}
        />
        <Typography
          variant="body1"
          sx={{
            color: client.contactT ? "inherit" : "gray",
          }}
        >
          Контакт: {client.contactT || "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <LocalPhone
          sx={{ mr: 1, color: client.telephoneT ? "inherit" : "gray" }}
        />
        <Typography
          variant="body1"
          sx={{
            color: client.telephoneT ? "inherit" : "gray",
          }}
        >
          Телефон: {client.telephoneT || "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <AlternateEmail
          sx={{ mr: 1, color: client.emailT ? "inherit" : "gray" }}
        />
        <Typography
          variant="body1"
          sx={{
            color: client.emailT ? "inherit" : "gray",
          }}
        >
          Email: {client.emailT || "Нет информации"}
        </Typography>
      </Box>
    </>
  );
}
