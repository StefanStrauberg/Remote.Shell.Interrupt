import { Box, Pagination, Typography } from "@mui/material";
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
  if (isLoading) return <Typography>Loading ...</Typography>;

  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value);
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 1fr)",
          gap: 3,
        }}
      >
        {tfPlans.map((tfPlan) => (
          <TfPlanCard key={tfPlan.id} tfPlan={tfPlan} />
        ))}
      </Box>

      <Pagination
        count={pagination.TotalPages}
        page={pageNumber}
        onChange={handlePageChange}
        variant="outlined"
        color="primary"
        sx={{ alignSelf: "center", mt: 2 }}
      />
    </Box>
  );
}
