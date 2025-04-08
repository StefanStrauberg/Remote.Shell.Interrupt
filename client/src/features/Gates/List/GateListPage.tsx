import { Box, Pagination, Typography } from "@mui/material";
import GateCard from "./GateCard";
import { Gate } from "../../../lib/types/Gate";
import { PaginationHeader } from "../../../lib/types/PaginationHeader";

type Props = {
  gates: Gate[] | undefined;
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationHeader;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function GateListPage({
  gates,
  isPending,
  pageNumber,
  pagination,
  setPageNumber,
}: Props) {
  // Loading state
  if (!gates || isPending) return <Typography>Loading ...</Typography>;

  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value); // Update the page number
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(2, 1fr)", // Two columns
          gap: 3,
        }}
      >
        {gates.map((gate) => (
          <GateCard key={gate.id} gate={gate} />
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
