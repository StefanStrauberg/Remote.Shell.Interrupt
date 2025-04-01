import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";
import { OrganizationShort } from "../../lib/types/OrganizationShort";

type Props = {
  organization: OrganizationShort;
};

export default function OrganizationCard({ organization }: Props) {
  return (
    <Card elevation={5} sx={{ borderRadius: 4, fontSize: 18 }}>
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>
            Имя: {organization.name}
          </Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Typography variant="body2">
            Контакт технический: {organization.contactT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Typography variant="body2">
            Телефон технический: {organization.telephoneT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Typography variant="body2">
            Email технический: {organization.emailT}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Typography variant="body2">
            Работает: {organization.working ? "Да" : "Нет"}
          </Typography>
        </Box>
        <Divider />
      </CardContent>
      <CardContent>
        <Box display="flex" justifyContent="space-between">
          <Box display="flex" alignItems="center">
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
