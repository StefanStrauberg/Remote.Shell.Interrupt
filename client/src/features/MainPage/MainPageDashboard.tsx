import { Grid2, Box, Typography, CircularProgress, Alert } from "@mui/material";
import MainPageListFilter from "./MainPageListFilter";
import MainPageList from "./MainPageList";
import { useState } from "react";
import { useRouters } from "../../lib/hooks/useRouters";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import SearchIcon from "@mui/icons-material/Search";

export default function MainPageDashboard() {
  const [filters, setFilters] = useState<RouterFilter>({});
  const [isEnabled, setEnabled] = useState(false);

  const { compoundObject, isCompoundObject, isError, error, resetCache } =
    useRouters(filters, isEnabled);

  const handleApplyFilters = (newFilters: RouterFilter) => {
    setFilters(newFilters);
    setEnabled(false);
  };

  const handleSearch = () => {
    resetCache();
    setEnabled(true);
  };

  // Show loading state
  if (isCompoundObject) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="400px"
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Searching for devices...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (isError) {
    return (
      <Box>
        <Alert severity="error" sx={{ mb: 2 }}>
          Error loading data:{" "}
          {error instanceof Error ? error.message : "Unknown error"}
        </Alert>
        <MainPageListFilter
          onApplyFilters={handleApplyFilters}
          onSearch={handleSearch}
        />
      </Box>
    );
  }

  return (
    <Grid2 container spacing={3}>
      <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
        {compoundObject ? (
          <MainPageList data={compoundObject} />
        ) : (
          <Box
            display="flex"
            flexDirection="column"
            alignItems="center"
            justifyContent="center"
            minHeight="300px"
          >
            <SearchIcon sx={{ fontSize: 64, color: "text.secondary", mb: 2 }} />
            <Typography variant="h6" color="text.secondary">
              Enter a VLAN ID to search for devices
            </Typography>
          </Box>
        )}
      </Grid2>

      <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
        <MainPageListFilter
          onApplyFilters={handleApplyFilters}
          onSearch={handleSearch}
        />
      </Grid2>
    </Grid2>
  );
}
