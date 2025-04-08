import { Box, Typography } from "@mui/material";
import { Client } from "../../../lib/types/Client";
import { Info } from "@mui/icons-material";

type Props = {
  client: Client;
};

export default function ClientDetailSPRVlans({ client }: Props) {
  return (
    <>
      {client.sprVlans && (
        <>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <Info
              sx={{
                mr: 1,
                color: client.sprVlans.length > 0 ? "#1976d2" : "gray",
              }}
            />
            <Typography
              sx={{ color: client.sprVlans.length > 0 ? "inherit" : "gray" }}
            >
              ID Влана:{" "}
              {client.sprVlans.length > 0
                ? client.sprVlans.map((vlan) => vlan.idVlan).join(", ")
                : "Нет информации"}
            </Typography>
          </Box>
        </>
      )}
    </>
  );
}
