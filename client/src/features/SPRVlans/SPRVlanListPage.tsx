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
  CircularProgress,
  Chip,
} from "@mui/material";
import { PaginationMetadata } from "../../lib/types/Common/PaginationMetadata";
import { SprVlan } from "../../lib/types/SPRVlans/SprVlan";
import { Link } from "react-router";
import { ArrowDownward, ArrowUpward, Sort } from "@mui/icons-material";
import { useState } from "react";

type Props = {
  sprVlans: SprVlan[];
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
  orderBy: string;
  orderByDescending: boolean;
  onSort: (property: string) => void;
};

const SORTABLE_FIELDS = [
  { id: "idVlan", label: "VLAN ID" },
  { id: "idClient", label: "Client ID" },
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
    { id: "idVlan", label: "VLAN ID" },
    { id: "idClient", label: "Client ID" },
    { id: "useClient", label: "Used by Client" },
    { id: "useCOD", label: "Used by COD" },
    { id: "actions", label: "Actions", sortable: false },
  ];

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
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        flexWrap="wrap"
        gap={2}
      >
        <Typography variant="h6" component="h2" gutterBottom>
          {pagination.TotalCount || 0} VLANs found
        </Typography>

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
                    <ArrowDownward fontSize="small" sx={{ ml: 1 }} />
                  ) : (
                    <ArrowUpward fontSize="small" sx={{ ml: 1 }} />
                  ))}
              </MenuItem>
            ))}
          </Menu>
        </Stack>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              {columns.map((column) => (
                <TableCell key={column.id}>{column.label}</TableCell>
              ))}
            </TableRow>
          </TableHead>

          <TableBody>
            {sprVlans.map((sprVlan) => (
              <TableRow key={sprVlan.id}>
                <TableCell>{sprVlan.idVlan}</TableCell>
                <TableCell>{sprVlan.idClient || "N/A"}</TableCell>
                <TableCell>
                  <Chip
                    label={sprVlan.useClient ? "Yes" : "No"}
                    color={sprVlan.useClient ? "success" : "default"}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <Chip
                    label={sprVlan.useCOD ? "Yes" : "No"}
                    color={sprVlan.useCOD ? "success" : "default"}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  {sprVlan.idClient && sprVlan.idClient !== 0 ? (
                    <Button
                      variant="outlined"
                      component={Link}
                      to={`/clients/${sprVlan.idClient}`}
                      size="small"
                    >
                      View Client
                    </Button>
                  ) : (
                    <Typography variant="body2" color="text.secondary">
                      No client
                    </Typography>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

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
