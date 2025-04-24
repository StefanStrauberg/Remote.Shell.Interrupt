import {
  Card,
  CardContent,
  CardHeader,
  Collapse,
  Divider,
  Typography,
} from "@mui/material";
import { Client } from "../../../lib/types/Clients/Client";
import ClientDetailGeneralInformation from "../../Clients/Detail/ClientDetailGeneralInformation";
import ClientDetailSPRVlans from "../../Clients/Detail/ClientDetailSPRVlans";
import ClientDetailTariffPlan from "../../Clients/Detail/ClientDetailTariffPlan";
import ClientDetailNotes from "../../Clients/Detail/ClientDetailNotes";
import ClientDetailCommercialContact from "../../Clients/Detail/ClientDetailCommercialContact";
import ClientDetailTechnicalContact from "../../Clients/Detail/ClientDetailTechnicalContact";
import ClientDetailHistory from "../../Clients/Detail/ClientDetailHistory";
import ClientDetailCOD from "../../Clients/Detail/ClientDetailCOD";
import { useState } from "react";

type Props = {
  client: Client;
};

export default function ClientBox({ client }: Props) {
  const [isExpanded, setExpanded] = useState(false); // Состояние для управления разворачиванием

  const toggleExpand = () => {
    setExpanded(!isExpanded); // Переключение состояния
  };

  return (
    <Card
      elevation={5}
      sx={{
        mb: 2,
        borderRadius: 4,
        boxShadow: 3,
        fontSize: 18,
        cursor: "pointer", // Визуально показывает, что по элементу можно кликнуть
      }}
      onClick={toggleExpand} // Добавляем обработчик клика
    >
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>
            {client.name} ({client.telephoneT || "к.т нет"},{" "}
            {client.emailT || "email нет"})
          </Typography>
        }
      />
      <Collapse in={isExpanded}>
        <CardContent sx={{ p: 2 }}></CardContent>
        <Divider>Общая информация</Divider>
        <ClientDetailGeneralInformation client={client} />
        <Divider>Вланы</Divider>
        <ClientDetailSPRVlans client={client} />
        <Divider>Тарифный план</Divider>
        <ClientDetailTariffPlan client={client} />
        <Divider>Примечания</Divider>
        <ClientDetailNotes client={client} />
        <Divider>Коммерческий контакт</Divider>
        <ClientDetailCommercialContact client={client} />
        <Divider>Технический контакт</Divider>
        <ClientDetailTechnicalContact client={client} />
        <Divider>История</Divider>
        <ClientDetailHistory client={client} />
        <Divider>ЦОД</Divider>
        <ClientDetailCOD client={client} />
      </Collapse>
    </Card>
  );
}
