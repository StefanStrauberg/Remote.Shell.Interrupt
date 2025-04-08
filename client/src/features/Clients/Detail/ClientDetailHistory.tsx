import { Box, Typography } from "@mui/material";
import { Client } from "../../../lib/types/Client";

type Props = {
  client: Client;
};

export default function ClientDetailHistory({ client }: Props) {
  return (
    <>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <Typography
          variant="body1"
          sx={{
            color: client.history ? "inherit" : "gray",
          }}
        >
          {client.history || "Нет информации"}
        </Typography>
      </Box>
    </>
  );
}
