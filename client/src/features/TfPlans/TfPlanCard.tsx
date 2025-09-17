import { Feed, Fingerprint } from "@mui/icons-material";
import {
  Box,
  Card,
  CardContent,
  CardHeader,
  Divider,
  Typography,
  Chip,
  Tooltip,
} from "@mui/material";
import { TfPlan } from "../../lib/types/TfPlans/TfPlan";

type Props = {
  tfPlan: TfPlan;
};

export default function TfPlanCard({ tfPlan }: Props) {
  return (
    <Card
      variant="outlined"
      sx={{
        borderRadius: 2,
        height: "100%",
        display: "flex",
        flexDirection: "column",
        transition: "all 0.2s ease-in-out",
        "&:hover": {
          boxShadow: 3,
          transform: "translateY(-2px)",
        },
      }}
    >
      <CardHeader
        title={
          <Tooltip title={tfPlan.nameTfPlan} arrow>
            <Typography
              variant="h6"
              component="h3"
              noWrap
              sx={{ fontWeight: "bold" }}
            >
              {tfPlan.nameTfPlan}
            </Typography>
          </Tooltip>
        }
        subheader={
          <Chip
            label={`ID: ${tfPlan.idTfPlan}`}
            size="small"
            color="primary"
            variant="outlined"
          />
        }
      />

      <Divider />

      <CardContent sx={{ flexGrow: 1, p: 2 }}>
        <Box display="flex" alignItems="flex-start" mb={2}>
          <Fingerprint sx={{ mr: 1, color: "text.secondary", mt: 0.25 }} />
          <Typography variant="body2">
            <strong>Plan ID:</strong> {tfPlan.idTfPlan}
          </Typography>
        </Box>

        <Box display="flex" alignItems="flex-start">
          <Feed sx={{ mr: 1, color: "text.secondary", mt: 0.25 }} />
          <Typography
            variant="body2"
            sx={{
              color: tfPlan.descTfPlan ? "inherit" : "text.secondary",
              wordBreak: "break-word",
            }}
          >
            <strong>Description:</strong>{" "}
            {tfPlan.descTfPlan || "No description available"}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
}
