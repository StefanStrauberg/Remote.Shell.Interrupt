import {
  AlternateEmail,
  CheckBox,
  ContactPage,
  Feed,
  Fingerprint,
  Info,
  LocalPhone,
} from "@mui/icons-material";
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
import { useClients } from "../../lib/hooks/useClients";

export default function ClientDetailPage() {
  const { id } = useParams();
  const { client, isLoadingOrganization } = useClients(0, 0, id);

  if (isLoadingOrganization) return <Typography>Loading...</Typography>;

  if (!client) return <Typography>Activity not found</Typography>;

  return (
    <Card elevation={5} sx={{ borderRadius: 4, boxShadow: 3, fontSize: 18 }}>
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>{client.name}</Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Fingerprint sx={{ mr: 1 }} />
          <Typography variant="body1" sx={{ color: "gray" }}>
            ID Клиента: {client.idClient}
          </Typography>
        </Box>
        <Divider>Коммерческий</Divider>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <ContactPage sx={{ mr: 1 }} />
          <Typography variant="body1">
            Контакт: {client.contactC || "Нет информации"}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <LocalPhone sx={{ mr: 1 }} />
          <Typography variant="body1">
            Телефон: {client.telephoneC || "Нет информации"}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <AlternateEmail sx={{ mr: 1 }} />
          <Typography variant="body1">
            Email: {client.emailC || "Нет информации"}
          </Typography>
        </Box>
        <Divider>Технический</Divider>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <ContactPage sx={{ mr: 1 }} />
          <Typography variant="body1">
            Контакт: {client.contactT || "Нет информации"}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <LocalPhone sx={{ mr: 1 }} />
          <Typography variant="body1">
            Телефон: {client.telephoneT || "Нет информации"}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <AlternateEmail sx={{ mr: 1 }} />
          <Typography variant="body1">
            Email: {client.emailT || "Нет информации"}
          </Typography>
        </Box>
        <Divider>История</Divider>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Typography variant="body1" sx={{ whiteSpace: "pre-wrap" }}>
            {client.history || "Нет информации"}
          </Typography>
        </Box>
        {/* cod Information */}
        {client.cod && (
          <>
            <Divider>ЦОД</Divider>
            <Box>
              <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
                <Info sx={{ mr: 1, color: "#1976d2" }} />
                <Typography variant="body1">
                  ЦОД: {client.cod.nameCOD} ({client.cod.region})
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" mb={2} px={2}>
                <AlternateEmail sx={{ mr: 1 }} />
                <Typography variant="body1">
                  Email 1: {client.cod.email1 || "Нет информации"}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" mb={2} px={2}>
                <AlternateEmail sx={{ mr: 1 }} />
                <Typography variant="body1">
                  Email 2: {client.cod.email2 || "Нет информации"}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" mb={2} px={2}>
                <LocalPhone sx={{ mr: 1 }} />
                <Typography variant="body1">
                  Контактный телефон: {client.cod.telephone || "Нет информации"}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" mb={2} px={2}>
                <Feed sx={{ mr: 1 }} />
                <Typography variant="body1">
                  Описание: {client.cod.description || "Нет информации"}
                </Typography>
              </Box>
            </Box>
          </>
        )}
        {/* tfPlan Information */}
        {client.tfPlan && (
          <>
            <Divider>Тарифный план</Divider>
            <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
              <Info sx={{ mr: 1, color: "#1976d2" }} />
              <Typography variant="body1">
                План: {client.tfPlan.nameTfPlan}{" "}
                {client.tfPlan.descTfPlan && `(${client.tfPlan.descTfPlan})`}
              </Typography>
            </Box>
          </>
        )}
        {/* Footer Section */}
        <Divider>Общая информация</Divider>
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
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <CheckBox
              sx={{ mr: 1, color: client.antiDDOS ? "green" : "red" }}
            />
            <Typography variant="body1">
              AntiDDOS: {client.antiDDOS ? "Да" : "Нет"}
            </Typography>
          </Box>
          <Button
            variant="contained"
            color="info"
            component={Link}
            to={`/clients`}
          >
            Back
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
