import { Grid2, Box, Button } from "@mui/material";
import { Link } from "react-router";
import PersonAddAltIcon from "@mui/icons-material/PersonAddAlt";
import { useState } from "react";
import GroupIcon from "@mui/icons-material/Group";
import UsersListPage from "./UsersListPage";
import UsersListFilter from "./UsersListFilter";
import { useUsersQuery } from "../api/usersQueries";
import { DEFAULT_USER_FILTERS } from "../api/usersApi";
import { useAuth } from "@/lib/auth/useAuth";
import EmptyPage from "@/app/shared/components/EmptyPage";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { DEFAULT_PAGINATION } from "@/lib/constants/pagination";
import { routes } from "@/app/router/paths";
import PageHeader from "@/app/shared/components/PageHeader";
import { PageError, PageLoading } from "@/app/shared/components/PageFeedback";

export default function UsersDashboard() {
  const { user: currentUser } = useAuth();
  const [filters, setFilters] =
    useState<FilterDescriptor[]>(DEFAULT_USER_FILTERS);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const pageSize = 15;
  const [orderBy] = useState<string>("Email");
  const [orderByDescending] = useState<boolean>(false);

  const usersQuery = useUsersQuery({
    pagination: { pageNumber, pageSize },
    filters,
    orderBy: { property: orderBy, descending: orderByDescending },
  });
  const users = usersQuery.data?.data ?? [];
  const pagination = usersQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const { isPending, error } = usersQuery;

  const handleApplyFilters = (newFilters: FilterDescriptor[]) => {
    setFilters(newFilters);
    setPageNumber(1);
  };

  const handleResetFilters = () => {
    setFilters(DEFAULT_USER_FILTERS);
    setPageNumber(1);
  };

  if (isPending) {
    return <PageLoading message="Loading accounts…" />;
  }

  if (error) {
    return <PageError title="Unable to load accounts" error={error} />;
  }

  return (
    <Box>
      <PageHeader
        title="User management"
        description="Review accounts, change roles, and activate or deactivate access"
        icon={GroupIcon}
        action={
          <Button
            component={Link}
            to={routes.register}
            variant="contained"
            startIcon={<PersonAddAltIcon />}
          >
            Create user
          </Button>
        }
      />

      <Grid2 container spacing={3}>
        <Grid2 size={{ xs: 12, md: 9 }} order={{ xs: 2, md: 1 }}>
          {users.length === 0 ? (
            <EmptyPage input="No accounts found" />
          ) : (
            <UsersListPage
              users={users}
              currentUserId={currentUser?.id}
              isPending={isPending}
              pageNumber={pageNumber}
              pagination={pagination}
              setPageNumber={setPageNumber}
            />
          )}
        </Grid2>
        <Grid2 size={{ xs: 12, md: 3 }} order={{ xs: 1, md: 2 }}>
          <UsersListFilter
            onApplyFilters={handleApplyFilters}
            initialFilters={filters}
            onResetFilters={handleResetFilters}
          />
        </Grid2>
      </Grid2>
    </Box>
  );
}
