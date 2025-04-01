import {
  AlternateEmail,
  CheckBox,
  ContactPage,
  ContactPhone,
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
import { useParams } from "react-router";
import { useOrganizations } from "../../lib/hooks/useOrganizations";

export default function OrganizationDetailPage() {
  const { id } = useParams();
  const { organization, isLoadingOrganization } = useOrganizations(0, 0, id);

  if (isLoadingOrganization) return <Typography>Loading...</Typography>;

  if (!organization) return <Typography>Activity not found</Typography>;

  return (
    <Card elevation={5} sx={{ borderRadius: 4, boxShadow: 3, fontSize: 18 }}>
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>
            {organization.name}
          </Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <ContactPage sx={{ mr: 1 }} />
          <Typography variant="body2">
            Контакт технический: {organization.contactT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <ContactPhone sx={{ mr: 1 }} />
          <Typography variant="body2">
            Телефон технический: {organization.telephoneT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <AlternateEmail sx={{ mr: 1 }} />
          <Typography variant="body2">
            Email технический: {organization.emailT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <CheckBox
            sx={{
              mr: 1,
              color: organization.working ? "green" : "red",
            }}
          />
          <Typography variant="body2">
            Работает: {organization.working ? "Да" : "Нет"}
          </Typography>
        </Box>
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <CheckBox
              sx={{ mr: 1, color: organization.antiDDOS ? "green" : "red" }}
            />
            <Typography variant="body2">
              AntiDDOS: {organization.antiDDOS ? "Да" : "Нет"}
            </Typography>
          </Box>
          <Button variant="contained">View</Button>
        </Box>
      </CardContent>
    </Card>
  );
}
