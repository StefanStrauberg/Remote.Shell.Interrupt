import {
  Box,
  Button,
  Menu,
  MenuItem,
  Pagination,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { PaginationMetadata } from "../../lib/types/Common/PaginationMetadata";
import { SprVlan } from "../../lib/types/SPRVlans/SprVlan";
import { Link } from "react-router";
import { ArrowDownward, ArrowUpward, Sort } from "@mui/icons-material";
import { useState } from "react";

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

const SORTABLE_FIELDS = [
  { id: "idVlan", label: "VLAN клиента" },
  { id: "idClient", label: "Id клиента" },
  // Add more fields as needed
];

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
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  if (!sprVlans || isPending) return <Typography>Загрузка ...</Typography>;

  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value);
  };

  const handleSortMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleSortMenuClose = () => {
    setAnchorEl(null);
  };

  const handleSortSelection = (property: string) => {
    onSort(property);
    handleSortMenuClose();
  };

  const columns = [
    { id: "idVlan", label: "VLAN клиента" },
    { id: "idClient", label: "Id клиента" },
    { id: "useClient", label: "Использ. клиентом" },
    { id: "useCOD", label: "Использ. ЦОД" },
    { id: "actions", label: "", sortable: false },
  ];

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* Sort controls */}
      <Stack
        direction="row"
        justifyContent="flex-end"
        alignItems="center"
        spacing={2}
      >
        <Button
          variant="outlined"
          startIcon={<Sort />}
          endIcon={orderByDescending ? <ArrowDownward /> : <ArrowUpward />}
          onClick={handleSortMenuClick}
        >
          Sort by:{" "}
          {SORTABLE_FIELDS.find((f) => f.id === orderBy)?.label || "Default"}
        </Button>

        <Menu anchorEl={anchorEl} open={open} onClose={handleSortMenuClose}>
          {SORTABLE_FIELDS.map((field) => (
            <MenuItem
              key={field.id}
              onClick={() => handleSortSelection(field.id)}
              selected={orderBy === field.id}
            >
              {field.label}
              {orderBy === field.id &&
                (orderByDescending ? (
                  <ArrowDownward fontSize="small" />
                ) : (
                  <ArrowUpward fontSize="small" />
                ))}
            </MenuItem>
          ))}
        </Menu>
      </Stack>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              {columns.map((column) => (
                <TableCell>{column.label}</TableCell>
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
