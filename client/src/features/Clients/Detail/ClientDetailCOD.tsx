import { Box, Typography } from "@mui/material";
import { AlternateEmail, Feed, Info, LocalPhone } from "@mui/icons-material";
import { Client } from "../../../lib/types/Client";

type Props = {
  client: Client;
};

export default function ClientDetailCOD({ client }: Props) {
  return (
    <>
      {client.cod && (
        <>
          <Box>
            <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
              <Info sx={{ mr: 1, color: "#1976d2" }} />
              <Typography variant="body1">
                ЦОД: {client.cod.nameCOD} ({client.cod.region})
              </Typography>
            </Box>
            <Box display="flex" alignItems="center" mb={2} px={2}>
              <AlternateEmail
                sx={{
                  mr: 1,
                  color: client.cod.email1 ? "inherit" : "gray",
                }}
              />
              <Typography
                variant="body1"
                sx={{
                  color: client.cod.email1 ? "inherit" : "gray",
                }}
              >
                Email 1: {client.cod.email1 || "Нет информации"}
              </Typography>
            </Box>
            <Box display="flex" alignItems="center" mb={2} px={2}>
              <AlternateEmail
                sx={{
                  mr: 1,
                  color: client.cod.email2 ? "inherit" : "gray",
                }}
              />
              <Typography
                variant="body1"
                sx={{
                  color: client.cod.email2 ? "inherit" : "gray",
                }}
              >
                Email 2: {client.cod.email2 || "Нет информации"}
              </Typography>
            </Box>
            <Box display="flex" alignItems="center" mb={2} px={2}>
              <LocalPhone
                sx={{
                  mr: 1,
                  color: client.cod.telephone ? "inherit" : "gray",
                }}
              />
              <Typography
                variant="body1"
                sx={{
                  color: client.cod.telephone ? "inherit" : "gray",
                }}
              >
                Контактный телефон: {client.cod.telephone || "Нет информации"}
              </Typography>
            </Box>
            <Box display="flex" alignItems="center" mb={2} px={2}>
              <Feed
                sx={{
                  mr: 1,
                  color: client.cod.description ? "inherit" : "gray",
                }}
              />
              <Typography
                variant="body1"
                sx={{
                  color: client.cod.description ? "inherit" : "gray",
                }}
              >
                Описание: {client.cod.description || "Нет информации"}
              </Typography>
            </Box>
          </Box>
        </>
      )}
    </>
  );
}
