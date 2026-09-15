import { Grid2, Box } from "@mui/material";
import MainPageListFilter from "./MainPageListFilter";
import MainPageList from "./MainPageList";
import { useState } from "react";
import { useRouterSearchQuery } from "./api/routerSearchQueries";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import SearchIcon from "@mui/icons-material/Search";
import PageHeader from "../../app/shared/components/PageHeader";
import EmptyPage from "../../app/shared/components/EmptyPage";
import {
  PageError,
  PageLoading,
} from "../../app/shared/components/PageFeedback";

export default function MainPageDashboard() {
  const [filters, setFilters] = useState<RouterFilter>({});
  const [isEnabled, setEnabled] = useState(false);

  const routerSearchQuery = useRouterSearchQuery(
    filters.IdVlan?.value,
    isEnabled
  );
  const compoundObject = routerSearchQuery.data;

  const handleApplyFilters = (newFilters: RouterFilter) => {
    setFilters(newFilters);
    setEnabled(false);
  };

  const handleSearch = () => {
    routerSearchQuery.clear();
    setEnabled(true);
  };

  if (routerSearchQuery.isLoading) {
    return <PageLoading message="Searching for devices…" />;
  }

  // Show error state
  if (routerSearchQuery.isError) {
    return (
      <Box>
        <PageError
          title="Unable to complete the search"
          error={routerSearchQuery.error}
        />
        <MainPageListFilter
          onApplyFilters={handleApplyFilters}
          onSearch={handleSearch}
        />
      </Box>
    );
  }

  return (
    <Box>
      <PageHeader
        title="Network search"
        description="Find customer and network-device details by VLAN ID"
        icon={SearchIcon}
      />
      <Grid2 container spacing={3}>
        <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
          {compoundObject ? (
            <MainPageList data={compoundObject} />
          ) : (
            <EmptyPage
              input="Start a VLAN search"
              description="Enter a VLAN ID in the filter panel to find connected devices and customer details."
            />
          )}
        </Grid2>

        <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
          <MainPageListFilter
            onApplyFilters={handleApplyFilters}
            onSearch={handleSearch}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
