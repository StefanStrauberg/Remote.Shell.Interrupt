import { Box, Typography } from "@mui/material";
import { Client } from "../../../lib/types/Clients/Client";
import { Note } from "@mui/icons-material";

type Props = {
  client: Client;
};

export default function ClientDetailNotes({ client }: Props) {
  return (
    <>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <Note sx={{ mr: 1, color: client.prim1 ? "inherit" : "gray" }} />
        <Typography
          variant="body1"
          sx={{
            color: client.prim1 ? "inherit" : "gray",
          }}
        >
          Примечание 1: {client.prim1 || "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <Note sx={{ mr: 1, color: client.prim2 ? "inherit" : "gray" }} />
        <Typography
          variant="body1"
          sx={{
            color: client.prim2 ? "inherit" : "gray",
          }}
        >
          Примечание 2: {client.prim2 || "Нет информации"}
        </Typography>
      </Box>
    </>
  );
}
