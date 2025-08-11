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
  TableSortLabel,
  Typography,
} from "@mui/material";
import { PaginationMetadata } from "../../lib/types/Common/PaginationMetadata";
import { SprVlan } from "../../lib/types/SPRVlans/SprVlan";
import { Link } from "react-router";

type Props = {
  sprVlans: SprVlan[] | undefined;
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
  orderBy: string;
  orderByDescending: boolean;
  onSort: (property: string) => void;
};

export default function SPRVlanListPage({
  sprVlans,
  isPending,
  pageNumber,
  pagination,
  setPageNumber,
  orderBy,
  orderByDescending,
  onSort,
}: Props) {
  if (!sprVlans || isPending) return <Typography>Загрузка ...</Typography>;

  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value);
  };

  const createSortHandler = (property: string) => () => {
    onSort(property);
  };

  const columns = [
    { id: "idVlan", label: "ID VLAN" },
    { id: "idClient", label: "ID Client" },
    { id: "useClient", label: "Use Client" },
    { id: "useCOD", label: "Use COD" },
    { id: "actions", label: "Actions", sortable: false },
  ];

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              {columns.map((column) => (
                <TableCell
                  key={column.id}
                  sortDirection={
                    orderBy === column.id
                      ? orderByDescending
                        ? "desc"
                        : "asc"
                      : false
                  }
                >
                  {column.sortable !== false ? (
                    <TableSortLabel
                      active={orderBy === column.id}
                      direction={
                        orderBy === column.id
                          ? orderByDescending
                            ? "desc"
                            : "asc"
                          : "asc"
                      }
                      onClick={createSortHandler(column.id)}
                    >
                      {column.label}
                    </TableSortLabel>
                  ) : (
                    column.label
                  )}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {sprVlans.map((sprVlan) => (
              <TableRow key={sprVlan.id}>
                <TableCell>{sprVlan.idVlan}</TableCell>
                <TableCell>{sprVlan.idClient}</TableCell>
                <TableCell>{sprVlan.useClient ? "Yes" : "No"}</TableCell>
                <TableCell>{sprVlan.useCOD ? "Yes" : "No"}</TableCell>
                <TableCell>
                  {sprVlan.idClient === 0 ? (
                    <></>
                  ) : (
                    <Button
                      variant="contained"
                      color="primary"
                      component={Link}
                      to={`/clients/${sprVlan.idClient}`}
                    >
                      клиент
                    </Button>
                  )}
                </TableCell>
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
