import {
  Box,
  Pagination,
  Typography,
  CircularProgress,
  Grid2,
} from "@mui/material";
import GateCard from "./GateCard";
import { Gate } from "../../../lib/types/Gates/Gate";
import { PaginationMetadata } from "../../../lib/types/Common/PaginationMetadata";

type Props = {
  gates: Gate[];
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function GateListPage({
  gates,
  isPending,
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
  if (isPending) {
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
        {pagination.TotalCount || 0} gates found
      </Typography>

      <Grid2 container spacing={2}>
        {gates.map((gate) => (
          <Grid2 size={{ xs: 12, sm: 6, lg: 4 }} key={gate.id}>
            <GateCard gate={gate} />
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
