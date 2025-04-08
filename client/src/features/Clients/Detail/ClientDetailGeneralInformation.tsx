import {
  CheckBox,
  Fingerprint,
  Gavel,
  QueryBuilder,
} from "@mui/icons-material";
import { Box, Typography } from "@mui/material";
import { Client } from "../../../lib/types/Client";
import { formatDate } from "../../../lib/utils";

type Props = {
  client: Client;
};

export default function ClientDetailGeneralInformation({ client }: Props) {
  return (
    <>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <Fingerprint sx={{ mr: 1 }} />
        <Typography variant="body1">ID Клиента: {client.idClient}</Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <Gavel sx={{ mr: 1 }} />
        <Typography variant="body1">
          Номер договора: {client.nrDogovor}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <QueryBuilder sx={{ mr: 1 }} />
        <Typography variant="body1">
          Дата начала:{" "}
          {client.dat1 === "0001-01-01T00:00:00"
            ? " "
            : formatDate(client.dat1) || "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <QueryBuilder sx={{ mr: 1 }} />
        <Typography variant="body1">
          Дата окончания:{" "}
          {client.dat2 === "0001-01-01T00:00:00"
            ? " "
            : formatDate(client.dat2) || "Нет информации"}
        </Typography>
      </Box>
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
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <CheckBox sx={{ mr: 1, color: client.antiDDOS ? "green" : "red" }} />
        <Typography variant="body1">
          AntiDDOS: {client.antiDDOS ? "Да" : "Нет"}
        </Typography>
      </Box>
    </>
  );
}
