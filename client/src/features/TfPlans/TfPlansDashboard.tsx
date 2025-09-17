import { useState } from "react";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { useTfPlans } from "../../lib/hooks/useTfPlans";
import EmptyPage from "../../app/shared/components/EmptyPage";
import { Grid2, Box, Typography, CircularProgress } from "@mui/material";
import TfPlanListPage from "./TfPlanListPage";
import PriceCheckIcon from "@mui/icons-material/PriceCheck";

export default function TfPlansDashboard() {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [orderBy] = useState<string>("idTfPlan");
  const [orderByDescending] = useState<boolean>(false);
  const pageSize = 12;
  const filters: FilterDescriptor[] = [];

  const { tfPlans, pagination, isLoading, error } = useTfPlans(
    { pageNumber, pageSize },
    filters,
    { property: orderBy, descending: orderByDescending }
  );

  // Show loading state
  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="400px"
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Loading tariff plans...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (error) {
    return (
      <Box>
        <Typography variant="h6" color="error" gutterBottom>
          Error loading tariff plans
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {error instanceof Error ? error.message : "An unknown error occurred"}
        </Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" alignItems="center" mb={3}>
        <PriceCheckIcon sx={{ mr: 1, fontSize: 32, color: "primary.main" }} />
        <Typography variant="h4" component="h1" fontWeight="bold">
          Tariff Plans
        </Typography>
      </Box>

      {tfPlans.length === 0 ? (
        <EmptyPage input="No tariff plans found" />
      ) : (
        <Grid2 container spacing={3}>
          <Grid2 size={12}>
            <TfPlanListPage
              tfPlans={tfPlans}
              isLoading={isLoading}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          </Grid2>
        </Grid2>
      )}
    </Box>
  );
}
