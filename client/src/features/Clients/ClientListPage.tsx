import { Box, Pagination, Typography } from "@mui/material";
import ClientCard from "./ClientCard";
import { ClientShort } from "../../lib/types/ClientShort";
import { PaginationHeader } from "../../lib/types/PaginationHeader";

type Props = {
  clients: ClientShort[] | undefined;
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationHeader;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function ClientListPage({
  clients,
  isPending,
  pageNumber,
  pagination,
  setPageNumber,
}: Props) {
  // Loading state
  if (!clients || isPending) return <Typography>Loading ...</Typography>;

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
        {clients.map((client) => (
          <ClientCard key={client.idClient} client={client} />
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
