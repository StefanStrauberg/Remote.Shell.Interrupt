import {
  Box,
  Button,
  Typography,
  Grid2,
  Alert,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Card,
  CardContent,
  Divider,
  Stack,
} from "@mui/material";
import { Link } from "react-router";
import { SvgIconComponent } from "@mui/icons-material";
import {
  useDeleteAllGatesMutation,
  useGatesQuery,
} from "../Gates/api/gatesQueries";
import {
  useDeleteAllClientsMutation,
  useSynchronizeClientsMutation,
} from "../Clients/api/clientsQueries";
import { useClientsQuery } from "../Clients/api/clientsQueries";
import { useNetworkDevicesQuery } from "../NetworkDevices/api/networkDevicesQueries";
import { useUsersQuery } from "../Users/api/usersQueries";
import { useState } from "react";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { DEFAULT_GATE_FILTERS } from "../Gates/api/gatesApi";
import { useDeleteAllNetworkDevicesMutation } from "../NetworkDevices/api/networkDevicesQueries";
import SettingsIcon from "@mui/icons-material/Settings";
import WarningIcon from "@mui/icons-material/Warning";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import RefreshIcon from "@mui/icons-material/Refresh";
import AddIcon from "@mui/icons-material/Add";
import RouterIcon from "@mui/icons-material/Router";
import GroupIcon from "@mui/icons-material/Group";
import PeopleAltIcon from "@mui/icons-material/PeopleAlt";
import DevicesIcon from "@mui/icons-material/Devices";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import { toast } from "react-toastify";
import { routes } from "../../app/router/paths";
import { DEFAULT_PAGINATION } from "../../lib/constants/pagination";
import PageHeader from "../../app/shared/components/PageHeader";

function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
}

type StatTileProps = {
  label: string;
  value: number | undefined;
  isLoading: boolean;
  icon: SvgIconComponent;
  color: "primary" | "success" | "info" | "warning";
};

function StatTile({
  label,
  value,
  isLoading,
  icon: Icon,
  color,
}: StatTileProps) {
  return (
    <Card variant="outlined" sx={{ height: "100%" }}>
      <CardContent sx={{ display: "flex", alignItems: "center", gap: 2 }}>
        <Box
          sx={{
            width: 48,
            height: 48,
            flexShrink: 0,
            display: "grid",
            placeItems: "center",
            borderRadius: 2.5,
            color: `${color}.main`,
            bgcolor: `${color}.light`,
          }}
        >
          <Icon />
        </Box>
        <Box>
          {isLoading ? (
            <CircularProgress size={22} />
          ) : (
            <Typography variant="h5" fontWeight={700}>
              {value ?? 0}
            </Typography>
          )}
          <Typography variant="body2" color="text.secondary">
            {label}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
}

type ManagementCardProps = {
  title: string;
  icon: SvgIconComponent;
  description: string;
  children: React.ReactNode;
  dangerZone?: React.ReactNode;
};

function ManagementCard({
  title,
  icon: Icon,
  description,
  children,
  dangerZone,
}: ManagementCardProps) {
  return (
    <Card variant="outlined" sx={{ height: "100%" }}>
      <CardContent
        sx={{
          display: "flex",
          flexDirection: "column",
          gap: 2,
          height: "100%",
        }}
      >
        <Stack direction="row" alignItems="center" gap={1}>
          <Icon color="primary" />
          <Typography variant="h6" component="h2">
            {title}
          </Typography>
        </Stack>

        <Typography variant="body2" color="text.secondary">
          {description}
        </Typography>

        <Box sx={{ flexGrow: 1 }}>{children}</Box>

        {dangerZone && (
          <>
            <Divider />
            <Box>
              <Typography
                variant="overline"
                color="error"
                sx={{ display: "flex", alignItems: "center", gap: 0.5, mb: 1 }}
              >
                <WarningIcon fontSize="small" /> Danger zone
              </Typography>
              {dangerZone}
            </Box>
          </>
        )}
      </CardContent>
    </Card>
  );
}

export default function AdminPage() {
  const pageNumber = 1;
  const pageSize = 50;
  const [gateFilters] = useState<FilterDescriptor[]>(DEFAULT_GATE_FILTERS);
  const [orderByForGates] = useState<string>("ipAddress");
  const [orderByDescending] = useState<boolean>(false);
  const [confirmDialog, setConfirmDialog] = useState<{
    open: boolean;
    title: string;
    message: string;
    action: () => void;
  }>({
    open: false,
    title: "",
    message: "",
    action: () => {},
  });

  const gatesQuery = useGatesQuery({
    pagination: { pageNumber, pageSize },
    filters: gateFilters,
    orderBy: {
      property: orderByForGates,
      descending: orderByDescending,
    },
  });
  const gatesPagination = gatesQuery.data?.pagination ?? DEFAULT_PAGINATION;
  const deleteAllGates = useDeleteAllGatesMutation();

  const deleteClients = useDeleteAllClientsMutation();
  const updateClients = useSynchronizeClientsMutation();

  const deleteNetworkDevices = useDeleteAllNetworkDevicesMutation();

  // Lightweight, unfiltered counts for the summary row - pageSize:1 means only
  // the X-Pagination header's TotalCount is actually used, not the row itself.
  const clientsCountQuery = useClientsQuery({
    pagination: { pageNumber: 1, pageSize: 1 },
    filters: [],
    orderBy: { property: "Name", descending: false },
  });
  const networkDevicesCountQuery = useNetworkDevicesQuery({
    pagination: { pageNumber: 1, pageSize: 1 },
    filters: [],
    orderBy: { property: "Host", descending: false },
  });
  const usersCountQuery = useUsersQuery({
    pagination: { pageNumber: 1, pageSize: 1 },
    filters: [],
    orderBy: { property: "Email", descending: false },
  });

  const showConfirmation = (
    title: string,
    message: string,
    action: () => void
  ) => {
    setConfirmDialog({
      open: true,
      title,
      message,
      action,
    });
  };

  const handleConfirm = () => {
    confirmDialog.action();
    setConfirmDialog({ open: false, title: "", message: "", action: () => {} });
  };

  const handleCancel = () => {
    setConfirmDialog({ open: false, title: "", message: "", action: () => {} });
  };

  const updateClientsHandle = () => {
    showConfirmation(
      "Update Clients",
      "Are you sure you want to update all clients? This operation may take some time.",
      () => {
        updateClients.mutate(undefined, {
          onSuccess: () => {
            toast.success("Clients were updated successfully.");
          },
          onError: (error) => {
            toast.error(`Failed to update clients: ${errorMessage(error)}`);
          },
        });
      }
    );
  };

  const deleteGatesHandle = () => {
    showConfirmation(
      "Delete All Gates",
      "WARNING: This will permanently delete ALL gates. This action cannot be undone!",
      () => {
        deleteAllGates.mutate(undefined, {
          onSuccess: (deletedCount) => {
            toast.success(`${deletedCount} gates were deleted successfully.`);
          },
          onError: (error) => {
            toast.error(`Failed to delete all gates: ${errorMessage(error)}`);
          },
        });
      }
    );
  };

  const deleteClientsHandle = () => {
    showConfirmation(
      "Delete All Clients",
      "WARNING: This will permanently delete ALL clients. This action cannot be undone!",
      () => {
        deleteClients.mutate(undefined, {
          onSuccess: () => {
            toast.success("Clients were deleted successfully.");
          },
          onError: (error) => {
            toast.error(`Failed to delete clients: ${errorMessage(error)}`);
          },
        });
      }
    );
  };

  const deleteAllNetworkDevices = () => {
    showConfirmation(
      "Delete All Network Devices",
      "WARNING: This will permanently delete ALL network devices. This action cannot be undone!",
      () => {
        deleteNetworkDevices.mutate(undefined, {
          onSuccess: () => {
            toast.success("All network devices were deleted successfully.");
          },
          onError: (error) => {
            toast.error(
              `Failed to delete all network devices: ${errorMessage(error)}`
            );
          },
        });
      }
    );
  };

  const isAnyOperationPending =
    deleteAllGates.isPending ||
    deleteClients.isPending ||
    updateClients.isPending ||
    deleteNetworkDevices.isPending ||
    gatesQuery.isLoading;

  return (
    <Box>
      <PageHeader
        title="Administration"
        description="Manage synchronization, network inventory, and system records"
        icon={SettingsIcon}
      />

      {isAnyOperationPending && (
        <Alert severity="info" sx={{ mb: 3 }}>
          <Box display="flex" alignItems="center">
            <CircularProgress size={20} sx={{ mr: 1 }} />
            Operation in progress...
          </Box>
        </Alert>
      )}

      {/* Summary */}
      <Grid2 container spacing={2} sx={{ mb: 3 }}>
        <Grid2 size={{ xs: 6, md: 3 }}>
          <StatTile
            label="Gates"
            value={gatesPagination.TotalCount}
            isLoading={gatesQuery.isLoading}
            icon={RouterIcon}
            color="primary"
          />
        </Grid2>
        <Grid2 size={{ xs: 6, md: 3 }}>
          <StatTile
            label="Clients"
            value={clientsCountQuery.data?.pagination.TotalCount}
            isLoading={clientsCountQuery.isLoading}
            icon={PeopleAltIcon}
            color="success"
          />
        </Grid2>
        <Grid2 size={{ xs: 6, md: 3 }}>
          <StatTile
            label="Network devices"
            value={networkDevicesCountQuery.data?.pagination.TotalCount}
            isLoading={networkDevicesCountQuery.isLoading}
            icon={DevicesIcon}
            color="info"
          />
        </Grid2>
        <Grid2 size={{ xs: 6, md: 3 }}>
          <StatTile
            label="User accounts"
            value={usersCountQuery.data?.pagination.TotalCount}
            isLoading={usersCountQuery.isLoading}
            icon={GroupIcon}
            color="warning"
          />
        </Grid2>
      </Grid2>

      <Grid2 container spacing={3} sx={{ alignItems: "stretch" }}>
        {/* Gates Section */}
        <Grid2 size={{ xs: 12, md: 6 }}>
          <ManagementCard
            title="Gate management"
            icon={RouterIcon}
            description="Gates are virtual entities created exclusively for polling gateways. The system automatically keeps them up to date."
            dangerZone={
              <Button
                variant="contained"
                color="error"
                onClick={deleteGatesHandle}
                startIcon={<DeleteForeverIcon />}
                disabled={
                  isAnyOperationPending || gatesPagination.TotalCount === 0
                }
                size="small"
              >
                Delete all gates ({gatesPagination.TotalCount})
              </Button>
            }
          >
            <Button
              variant="contained"
              color="primary"
              component={Link}
              to={routes.createGate}
              startIcon={<AddIcon />}
              disabled={isAnyOperationPending}
            >
              Create gate
            </Button>
          </ManagementCard>
        </Grid2>

        {/* Clients Section */}
        <Grid2 size={{ xs: 12, md: 6 }}>
          <ManagementCard
            title="Client management"
            icon={PeopleAltIcon}
            description="Clients are entities whose information is collected from the billing system."
            dangerZone={
              <Button
                variant="contained"
                color="error"
                onClick={deleteClientsHandle}
                startIcon={<DeleteForeverIcon />}
                disabled={isAnyOperationPending}
                size="small"
              >
                Delete all clients
              </Button>
            }
          >
            <Button
              variant="contained"
              color="warning"
              onClick={updateClientsHandle}
              startIcon={<RefreshIcon />}
              disabled={isAnyOperationPending}
            >
              Sync clients, plans, VLANs
            </Button>
          </ManagementCard>
        </Grid2>

        {/* Network Devices Section */}
        <Grid2 size={{ xs: 12, md: 6 }}>
          <ManagementCard
            title="Network device management"
            icon={DevicesIcon}
            description="Network devices are entities whose information is collected from data center routers over SNMP."
            dangerZone={
              <Button
                variant="contained"
                color="error"
                onClick={deleteAllNetworkDevices}
                startIcon={<DeleteForeverIcon />}
                disabled={isAnyOperationPending}
                size="small"
              >
                Delete all network devices
              </Button>
            }
          >
            <Button
              component={Link}
              to={routes.networkDevices}
              variant="contained"
              color="info"
              disabled={isAnyOperationPending}
            >
              View devices
            </Button>
          </ManagementCard>
        </Grid2>

        {/* Users Section */}
        <Grid2 size={{ xs: 12, md: 6 }}>
          <ManagementCard
            title="User management"
            icon={GroupIcon}
            description="Review accounts, change roles, and activate or deactivate access. New accounts are created via the Register page."
          >
            <Button
              component={Link}
              to={routes.adminUsers}
              variant="contained"
              color="secondary"
              startIcon={<ManageAccountsIcon />}
            >
              Manage users
            </Button>
          </ManagementCard>
        </Grid2>
      </Grid2>

      {/* Confirmation Dialog */}
      <Dialog
        open={confirmDialog.open}
        onClose={handleCancel}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          <WarningIcon
            color="warning"
            sx={{ mr: 1, verticalAlign: "bottom" }}
          />
          {confirmDialog.title}
        </DialogTitle>
        <DialogContent>
          <Typography variant="body1" sx={{ mt: 1 }}>
            {confirmDialog.message}
          </Typography>
          <Alert severity="error" sx={{ mt: 2 }}>
            This action cannot be undone. Please make sure you understand the
            consequences.
          </Alert>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCancel} disabled={isAnyOperationPending}>
            Cancel
          </Button>
          <Button
            onClick={handleConfirm}
            color="error"
            variant="contained"
            disabled={isAnyOperationPending}
            startIcon={
              isAnyOperationPending ? (
                <CircularProgress size={20} />
              ) : (
                <DeleteForeverIcon />
              )
            }
          >
            {isAnyOperationPending ? "Processing..." : "Confirm"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
