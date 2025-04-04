import {
  AlternateEmail,
  CheckBox,
  ContactPage,
  Feed,
  Fingerprint,
  Gavel,
  Info,
  LocalPhone,
  Note,
  QueryBuilder,
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
import { formatDate } from "../../lib/utils";

export default function ClientDetailPage() {
  const { id } = useParams();
  const { client, isLoadingClient } = useClients(0, 0, id);

  if (isLoadingClient) return <Typography>Loading...</Typography>;

  if (!client) return <Typography>Activity not found</Typography>;

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
            <Typography sx={{ fontWeight: "bold" }}>{client.name}</Typography>
          }
        />
        <Divider>Общая информация</Divider>
        <CardContent sx={{ p: 0 }}>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <Fingerprint sx={{ mr: 1 }} />
            <Typography variant="body1">
              ID Клиента: {client.idClient}
            </Typography>
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
              Дата начала: {formatDate(client.dat1) || "Нет информации"}
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
            <CheckBox
              sx={{ mr: 1, color: client.antiDDOS ? "green" : "red" }}
            />
            <Typography variant="body1">
              AntiDDOS: {client.antiDDOS ? "Да" : "Нет"}
            </Typography>
          </Box>
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
          <Divider>Примечания</Divider>
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
          <Divider>Коммерческий</Divider>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <ContactPage
              sx={{ mr: 1, color: client.contactC ? "inherit" : "gray" }}
            />
            <Typography
              variant="body1"
              sx={{
                color: client.contactC ? "inherit" : "gray",
              }}
            >
              Контакт: {client.contactC || "Нет информации"}
            </Typography>
          </Box>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <LocalPhone
              sx={{ mr: 1, color: client.telephoneC ? "inherit" : "gray" }}
            />
            <Typography
              variant="body1"
              sx={{
                color: client.telephoneC ? "inherit" : "gray",
              }}
            >
              Телефон: {client.telephoneC || "Нет информации"}
            </Typography>
          </Box>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <AlternateEmail
              sx={{ mr: 1, color: client.emailC ? "inherit" : "gray" }}
            />
            <Typography
              variant="body1"
              sx={{
                color: client.emailC ? "inherit" : "gray",
              }}
            >
              Email: {client.emailC || "Нет информации"}
            </Typography>
          </Box>
          <Divider>Технический</Divider>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <ContactPage
              sx={{ mr: 1, color: client.contactT ? "inherit" : "gray" }}
            />
            <Typography
              variant="body1"
              sx={{
                color: client.contactT ? "inherit" : "gray",
              }}
            >
              Контакт: {client.contactT || "Нет информации"}
            </Typography>
          </Box>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <LocalPhone
              sx={{ mr: 1, color: client.telephoneT ? "inherit" : "gray" }}
            />
            <Typography
              variant="body1"
              sx={{
                color: client.telephoneT ? "inherit" : "gray",
              }}
            >
              Телефон: {client.telephoneT || "Нет информации"}
            </Typography>
          </Box>
          <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
            <AlternateEmail
              sx={{ mr: 1, color: client.emailT ? "inherit" : "gray" }}
            />
            <Typography
              variant="body1"
              sx={{
                color: client.emailT ? "inherit" : "gray",
              }}
            >
              Email: {client.emailT || "Нет информации"}
            </Typography>
          </Box>
          <Divider>История</Divider>
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
                    Контактный телефон:{" "}
                    {client.cod.telephone || "Нет информации"}
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
        </CardContent>
      </Card>
    </Box>
  );
}
