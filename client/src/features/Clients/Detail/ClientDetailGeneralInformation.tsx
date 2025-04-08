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
  const dat1 = client.dat1 ? new Date(client.dat1) : undefined;
  const dat2 = client.dat2 ? new Date(client.dat2) : undefined;
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
        <QueryBuilder
          sx={{
            mr: 1,
            color:
              dat1?.getTime() === new Date("0000-12-31T22:09:44.000Z").getTime()
                ? "gray"
                : "inherit",
          }}
        />
        <Typography
          variant="body1"
          sx={{
            color:
              dat1?.getTime() === new Date("0000-12-31T22:09:44.000Z").getTime()
                ? "gray"
                : "inherit",
          }}
        >
          Дата начала:{" "}
          {dat1
            ? dat1.getTime() === new Date("0000-12-31T22:09:44.000Z").getTime()
              ? " "
              : formatDate(dat1)
            : "Нет информации"}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
        <QueryBuilder
          sx={{
            mr: 1,
            color:
              dat2?.getTime() === new Date("0000-12-31T22:09:44.000Z").getTime()
                ? "gray"
                : "inherit",
          }}
        />
        <Typography
          variant="body1"
          sx={{
            color:
              dat2?.getTime() === new Date("0000-12-31T22:09:44.000Z").getTime()
                ? "gray"
                : "inherit",
          }}
        >
          Дата окончания:{" "}
          {dat2
            ? dat2.getTime() === new Date("0000-12-31T22:09:44.000Z").getTime()
              ? " "
              : formatDate(dat2)
            : "Нет информации"}
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
