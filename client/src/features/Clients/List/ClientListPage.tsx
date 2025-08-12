import {
  Box,
  Button,
  Menu,
  MenuItem,
  Pagination,
  Stack,
  Typography,
} from "@mui/material";
import ClientCard from "./ClientCard";
import { ClientShort } from "../../../lib/types/Clients/ClientShort";
import { PaginationMetadata } from "../../../lib/types/Common/PaginationMetadata";
import { useState } from "react";
import { ArrowDownward, ArrowUpward, Sort } from "@mui/icons-material";

type Props = {
  clients: ClientShort[] | undefined;
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
  orderBy: string;
  orderByDescending: boolean;
  onSort: (property: string) => void;
};

const SORTABLE_FIELDS = [
  { id: "name", label: "Имя клиента" },
  { id: "idClient", label: "Id клиента" },
  { id: "nrDogovor", label: "Номер договора" },
  // Add more fields as needed
];

export default function ClientListPage({
  clients,
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

  // Loading state
  if (!clients || isPending) return <Typography>Loading ...</Typography>;

  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value); // Update the page number
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

      {/* Client cards grid */}
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
