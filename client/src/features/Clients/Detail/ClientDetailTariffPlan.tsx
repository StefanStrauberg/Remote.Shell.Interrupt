import { Info } from "@mui/icons-material";
import { Box, Typography } from "@mui/material";
import { Client } from "../../../lib/types/Client";

type Props = {
  client: Client;
};

export default function ClientDetailTariffPlan({ client }: Props) {
  return (
    <>
      {client.tfPlan && (
        <>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <Info sx={{ mr: 1, color: "#1976d2" }} />
            <Typography variant="body1">
              План: {client.tfPlan.nameTfPlan}{" "}
              {client.tfPlan.descTfPlan && `(${client.tfPlan.descTfPlan})`}
            </Typography>
          </Box>
        </>
      )}
    </>
  );
}
