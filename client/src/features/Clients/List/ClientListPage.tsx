import {
  Box,
  Button,
  Menu,
  MenuItem,
  Pagination,
  Stack,
  Typography,
  CircularProgress,
  Chip,
} from "@mui/material";
import ClientCard from "./ClientCard";
import { ClientShort } from "../../../lib/types/Clients/ClientShort";
import { PaginationMetadata } from "../../../lib/types/Common/PaginationMetadata";
import { useState } from "react";
import {
  ArrowDownward,
  ArrowUpward,
  Sort,
  GridView,
  ViewList,
} from "@mui/icons-material";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";

type Props = {
  clients: ClientShort[];
  isPending: boolean;
  pageNumber: number;
  pagination: PaginationMetadata;
  setPageNumber: (value: React.SetStateAction<number>) => void;
  orderBy: string;
  orderByDescending: boolean;
  onSort: (property: string) => void;
};

const SORTABLE_FIELDS = [
  { id: "name", label: "Client Name" },
  { id: "idClient", label: "Client ID" },
  { id: "nrDogovor", label: "Contract Number" },
];

type ViewMode = "grid" | "list";

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
  const [viewMode, setViewMode] = useState<ViewMode>("grid");
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

  const handleViewModeChange = (
    _event: React.MouseEvent<HTMLElement>,
    newViewMode: ViewMode
  ) => {
    if (newViewMode !== null) {
      setViewMode(newViewMode);
    }
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
      {/* Controls */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        flexWrap="wrap"
        gap={2}
      >
        <Typography variant="h6" component="h2">
          {pagination.TotalCount || 0} clients found
        </Typography>

        <Stack direction="row" spacing={1} alignItems="center">
          <ToggleButtonGroup
            value={viewMode}
            exclusive
            onChange={handleViewModeChange}
            aria-label="view mode"
            size="small"
          >
            <ToggleButton value="grid" aria-label="grid view">
              <GridView />
            </ToggleButton>
            <ToggleButton value="list" aria-label="list view">
              <ViewList />
            </ToggleButton>
          </ToggleButtonGroup>

          <Button
            variant="outlined"
            startIcon={<Sort />}
            onClick={handleSortMenuClick}
            size="small"
          >
            Sort
            {orderBy && (
              <Chip
                label={
                  SORTABLE_FIELDS.find((f) => f.id === orderBy)?.label ||
                  orderBy
                }
                size="small"
                sx={{ ml: 1 }}
                variant="outlined"
                icon={orderByDescending ? <ArrowDownward /> : <ArrowUpward />}
              />
            )}
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

      {/* Client cards grid/list */}
      <Box
        sx={{
          display: viewMode === "grid" ? "grid" : "flex",
          gridTemplateColumns:
            viewMode === "grid"
              ? { xs: "1fr", sm: "repeat(2, 1fr)", lg: "repeat(3, 1fr)" }
              : "1fr",
          flexDirection: viewMode === "list" ? "column" : undefined,
          gap: 2,
        }}
      >
        {clients.map((client) => (
          <ClientCard
            key={client.idClient}
            client={client}
            viewMode={viewMode}
          />
        ))}
      </Box>

      {/* Pagination */}
      {pagination.TotalPages > 1 && (
        <Box display="flex" justifyContent="center" mt={3}>
          <Pagination
            count={pagination.TotalPages}
            page={pageNumber}
            onChange={handlePageChange}
            color="primary"
            showFirstButton
            showLastButton
            size="large"
          />
        </Box>
      )}
    </Box>
  );
}
