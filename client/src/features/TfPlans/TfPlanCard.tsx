import { Feed, Fingerprint } from "@mui/icons-material";
import {
  Box,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
} from "@mui/material";

import { TfPlan } from "../../lib/types/TfPlan";

type Props = {
  tfPlan: TfPlan;
};

export default function TfPlanCard({ tfPlan }: Props) {
  return (
    <Card elevation={5} sx={{ borderRadius: 4, boxShadow: 3, fontSize: 18 }}>
      <CardHeader
        title={
          <Typography sx={{ fontWeight: "bold" }}>
            {tfPlan.nameTfPlan}
          </Typography>
        }
      />
      <Divider />
      <CardContent sx={{ p: 0 }}>
        <Box display="flex" alignItems="center" mt={2} mb={2} px={2}>
          <Fingerprint sx={{ mr: 1 }} />
          <Typography variant="body1" sx={{ color: "gray" }}>
            ID Тарифного плана: {tfPlan.idTfPlan}
          </Typography>
        </Box>
        <Divider />
        <Box display="flex" alignItems="center" mt={2} px={2}>
          <Feed sx={{ mr: 1 }} />
          <Typography
            variant="body1"
            sx={{
              color: tfPlan.descTfPlan ? "inherit" : "gray", // Conditionally apply gray color
            }}
          >
            Описание: {tfPlan.descTfPlan || "Нет информации"}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
}
