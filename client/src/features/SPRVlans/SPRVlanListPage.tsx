import {
  Box,
  Button,
  Pagination,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { PaginationHeader } from "../../lib/types/PaginationHeader";
import { SprVlan } from "../../lib/types/SprVlan";

type Props = {
  sprVlans: SprVlan[] | undefined;
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationHeader;
  setPageNumber: (value: React.SetStateAction<number>) => void;
};

export default function SPRVlanListPage({
  sprVlans,
  isPending,
  pageNumber,
  pagination,
  setPageNumber,
}: Props) {
  // Loading state
  if (!sprVlans || isPending) return <Typography>Загрузка ...</Typography>;

  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value); // Update the page number
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID VLAN</TableCell>
              <TableCell>ID Client</TableCell>
              <TableCell>Use Client</TableCell>
              <TableCell>Use COD</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sprVlans.map((sprVlan) => (
              <TableRow key={sprVlan.id}>
                <TableCell>{sprVlan.idVlan}</TableCell>
                <TableCell>
                  {sprVlan.idClient === 0 ? (
                    sprVlan.idClient
                  ) : (
                    <Button variant="contained" color="primary">
                      {sprVlan.idClient}
                    </Button>
                  )}
                </TableCell>
                <TableCell>{sprVlan.useClient ? "Yes" : "No"}</TableCell>
                <TableCell>{sprVlan.useCOD ? "Yes" : "No"}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
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
