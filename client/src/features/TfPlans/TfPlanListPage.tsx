import {
  Box,
  Pagination,
  Typography,
  CircularProgress,
  Grid2,
} from "@mui/material";
import TfPlanCard from "./TfPlanCard";
import { TfPlan } from "../../lib/types/TfPlans/TfPlan";
import { PaginationMetadata } from "../../lib/types/Common/PaginationMetadata";

type Props = {
  tfPlans: TfPlan[];
  isLoading: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function TfPlanListPage({
  tfPlans,
  isLoading,
  pageNumber,
  pagination,
  setPageNumber,
}: Props) {
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value);
  };

  // Show loading state
  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="200px"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Typography variant="h6" component="h2" gutterBottom>
        {pagination.TotalCount || 0} tariff plans found
      </Typography>

      <Grid2 container spacing={2}>
        {tfPlans.map((tfPlan) => (
          <Grid2 size={{ xs: 12, sm: 6, md: 4 }} key={tfPlan.id}>
            <TfPlanCard tfPlan={tfPlan} />
          </Grid2>
        ))}
      </Grid2>

      {pagination.TotalPages > 1 && (
        <Box display="flex" justifyContent="center" mt={3}>
          <Pagination
            count={pagination.TotalPages}
            page={pageNumber}
            onChange={handlePageChange}
            color="primary"
            showFirstButton
            showLastButton
          />
        </Box>
      )}
    </Box>
  );
}
