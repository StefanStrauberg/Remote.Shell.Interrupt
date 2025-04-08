import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { Link, useParams } from "react-router";
import { useClients } from "../../../lib/hooks/useClients";
import ClientDetailGeneralInformation from "./ClientDetailGeneralInformation";
import ClientDetailTariffPlan from "./ClientDetailTariffPlan";
import ClientDetailNotes from "./ClientDetailNotes";
import ClientDetailCommercialContact from "./ClientDetailCommercialContact";
import ClientDetailTechnicalContact from "./ClientDetailTechnicalContact";
import ClientDetailHistory from "./ClientDetailHistory";
import ClientDetailCOD from "./ClientDetailCOD";

export default function ClientDetailPage() {
  const { id } = useParams();
  const { clientById, isLoadingById } = useClients(0, 0, {}, id);

  if (isLoadingById) return <Typography>Loading...</Typography>;

  if (!clientById) return <Typography>Activity not found</Typography>;

  return (
    <Box>
      <Box display="flex" mt={2} mb={2}>
        <Button
          variant="contained"
          color="info"
          component={Link}
          to={`/clients`}
        >
          Назад
        </Button>
      </Box>
      <Card elevation={5} sx={{ borderRadius: 4, boxShadow: 3, fontSize: 18 }}>
        <CardHeader
          title={
            <Typography sx={{ fontWeight: "bold" }}>
              {clientById.name}
            </Typography>
          }
        />
        <CardContent sx={{ p: 0 }}>
          <Divider>Общая информация</Divider>
          <ClientDetailGeneralInformation client={clientById} />
          <Divider>Тарифный план</Divider>
          <ClientDetailTariffPlan client={clientById} />
          <Divider>Примечания</Divider>
          <ClientDetailNotes client={clientById} />
          <Divider>Коммерческий контакт</Divider>
          <ClientDetailCommercialContact client={clientById} />
          <Divider>Технический контакт</Divider>
          <ClientDetailTechnicalContact client={clientById} />
          <Divider>История</Divider>
          <ClientDetailHistory client={clientById} />
          <Divider>ЦОД</Divider>
          <ClientDetailCOD client={clientById} />
        </CardContent>
      </Card>
    </Box>
  );
}
