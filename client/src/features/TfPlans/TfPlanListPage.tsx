import { Box, Pagination, Typography } from "@mui/material";
import TfPlanCard from "./TfPlanCard";
import { TfPlan } from "../../lib/types/TfPlans/TfPlan";
import { PaginationHeader } from "../../lib/types/Common/PaginationHeader";

type Props = {
  tfPlans: TfPlan[] | undefined;
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationHeader;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function TfPlanListPage({
  tfPlans,
  isPending,
  pageNumber,
  pagination,
  setPageNumber,
}: Props) {
  // Loading state
  if (!tfPlans || isPending) return <Typography>Loading ...</Typography>;

  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value); // Update the page number
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* Render tfPlans */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 1fr)", // Two columns
          gap: 3,
        }}
      >
        {tfPlans.map((tfPlan) => (
          <TfPlanCard key={tfPlan.id} tfPlan={tfPlan} />
        ))}
      </Box>

      {/* Pagination Component */}
      <Pagination
        count={pagination.TotalPages || 1} // Total pages based on pagination metadata
        page={pageNumber} // Current active page
        onChange={handlePageChange} // Handle page change
        variant="outlined"
        color="primary"
        sx={{ alignSelf: "center", mt: 2 }}
      />
    </Box>
  );
}
