import { Box, Button, Paper, Stack, Typography } from "@mui/material";
import { Search } from "@mui/icons-material";
import { Link, useSearchParams } from "react-router";
import { useState } from "react";
import { routes } from "../../app/router/paths";
import MainPageListFilter from "./MainPageListFilter";
import MainPageList from "./MainPageList";
import { useRouterSearchQuery } from "./api/routerSearchQueries";
import { RouterFilter } from "../../lib/types/NetworkDevices/RouterFilter";
import { isValidVlanId } from "../../lib/utils";
import PageHeader from "../../app/shared/components/PageHeader";
import EmptyPage from "../../app/shared/components/EmptyPage";
import {
  PageError,
  PageLoading,
} from "../../app/shared/components/PageFeedback";

export default function MainPageDashboard() {
  const [params, setParams] = useSearchParams();
  const value = Number(params.get("vlan"));
  const initialVlan = isValidVlanId(value) ? value : undefined;
  return (
    <SearchWorkspace
      key={initialVlan ?? "empty"}
      initialVlan={initialVlan}
      onChange={(vlan) => setParams(vlan ? { vlan: String(vlan) } : {})}
    />
  );
}
function SearchWorkspace({
  initialVlan,
  onChange,
}: {
  initialVlan?: number;
  onChange: (vlan?: number) => void;
}) {
  const [filters, setFilters] = useState<RouterFilter>(
    initialVlan ? { IdVlan: { op: "==", value: initialVlan } } : {}
  );
  const [enabled, setEnabled] = useState(!!initialVlan);
  const query = useRouterSearchQuery(filters.IdVlan?.value, enabled);
  return (
    <Box>
      <PageHeader
        title="Network search"
        description="Find customers and interfaces by VLAN. Inspect a port without leaving your results."
        icon={Search}
      />
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <MainPageListFilter
          initialVlan={initialVlan}
          onApplyFilters={(next) => {
            setFilters(next);
            setEnabled(false);
            onChange(next.IdVlan?.value);
          }}
          onSearch={() => {
            query.clear();
            setEnabled(true);
          }}
        />
      </Paper>
      {query.isLoading ? (
        <PageLoading message="Searching for devices…" />
      ) : query.isError ? (
        <PageError title="Unable to complete the search" error={query.error} />
      ) : query.data ? (
        <MainPageList data={query.data} />
      ) : (
        <EmptyPage
          input="Start a VLAN search"
          description="Enter a VLAN ID above to find devices, interfaces and customer details."
        />
      )}
      <Stack
        direction="row"
        alignItems="center"
        gap={1}
        flexWrap="wrap"
        sx={{ mt: 3 }}
      >
        <Typography variant="caption" color="text.secondary">
          Browse directories
        </Typography>
        <Button size="small" component={Link} to={routes.networkDevices}>
          Network devices
        </Button>
        <Button size="small" component={Link} to={routes.clients}>
          Customers
        </Button>
        <Button size="small" component={Link} to={routes.sprVlans}>
          VLAN inventory
        </Button>
      </Stack>
    </Box>
  );
}
