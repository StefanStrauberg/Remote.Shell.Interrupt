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
} from "@mui/material";
import { Link } from "react-router";
import {
  useDeleteAllGatesMutation,
  useGatesQuery,
} from "../Gates/api/gatesQueries";
import {
  useDeleteAllClientsMutation,
  useSynchronizeClientsMutation,
} from "../Clients/api/clientsQueries";
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
import { toast } from "react-toastify";
import { routes } from "../../app/router/paths";
import { DEFAULT_PAGINATION } from "../../lib/constants/pagination";
import PageHeader from "../../app/shared/components/PageHeader";

function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
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

      <Grid2 container spacing={3}>
        {/* Gates Section */}
        <Grid2 size={12}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h6" gutterBottom color="primary">
                <RouterIcon sx={{ mr: 1, verticalAlign: "bottom" }} />
                Gate Management
              </Typography>

              <Grid2 container spacing={2} alignItems="center" sx={{ mb: 2 }}>
                <Grid2 size="auto">
                  <Button
                    variant="contained"
                    color="primary"
                    component={Link}
                    to={routes.createGate}
                    startIcon={<AddIcon />}
                    disabled={isAnyOperationPending}
                  >
                    Create Gate
                  </Button>
                </Grid2>
                <Grid2>
                  <Typography variant="body2" color="text.secondary">
                    Create a new gate router. The system will automatically
                    update information about them.
                  </Typography>
                </Grid2>
              </Grid2>

              <Grid2 container spacing={2} alignItems="center">
                <Grid2 size="auto">
                  <Button
                    variant="contained"
                    color="error"
                    onClick={deleteGatesHandle}
                    startIcon={<DeleteForeverIcon />}
                    disabled={
                      isAnyOperationPending || gatesPagination.TotalCount === 0
                    }
                  >
                    Delete All Gates
                  </Button>
                </Grid2>
                <Grid2>
                  <Typography variant="body2" color="text.secondary">
                    <WarningIcon
                      sx={{
                        fontSize: 16,
                        verticalAlign: "text-bottom",
                        mr: 0.5,
                      }}
                    />
                    Warning! This will permanently delete all gates (
                    {gatesPagination.TotalCount} found).
                  </Typography>
                </Grid2>
              </Grid2>

              <Typography
                variant="body2"
                color="text.primary"
                sx={{ mt: 2, fontStyle: "italic" }}
              >
                Gates are virtual entities created exclusively for polling
                gateways.
              </Typography>
            </CardContent>
          </Card>
        </Grid2>

        {/* Clients Section */}
        <Grid2 size={12}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h6" gutterBottom color="primary">
                👥 Client Management
              </Typography>

              <Grid2 container spacing={2} alignItems="center" sx={{ mb: 2 }}>
                <Grid2 size="auto">
                  <Button
                    variant="contained"
                    color="warning"
                    onClick={updateClientsHandle}
                    startIcon={<RefreshIcon />}
                    disabled={isAnyOperationPending}
                  >
                    Update Clients
                  </Button>
                </Grid2>
                <Grid2>
                  <Typography variant="body2" color="text.secondary">
                    Update information about clients, tariff plans, VLANs, and
                    address pools.
                  </Typography>
                </Grid2>
              </Grid2>

              <Grid2 container spacing={2} alignItems="center">
                <Grid2 size="auto">
                  <Button
                    variant="contained"
                    color="error"
                    onClick={deleteClientsHandle}
                    startIcon={<DeleteForeverIcon />}
                    disabled={isAnyOperationPending}
                  >
                    Delete All Clients
                  </Button>
                </Grid2>
                <Grid2>
                  <Typography variant="body2" color="text.secondary">
                    <WarningIcon
                      sx={{
                        fontSize: 16,
                        verticalAlign: "text-bottom",
                        mr: 0.5,
                      }}
                    />
                    Warning! This will permanently delete all clients.
                  </Typography>
                </Grid2>
              </Grid2>

              <Typography
                variant="body2"
                color="text.primary"
                sx={{ mt: 2, fontStyle: "italic" }}
              >
                Clients are entities whose information is collected from the
                billing system.
              </Typography>
            </CardContent>
          </Card>
        </Grid2>

        {/* Network Devices Section */}
        <Grid2 size={12}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h6" gutterBottom color="primary">
                🌐 Network Devices Management
              </Typography>

              <Grid2 container spacing={2} alignItems="center" sx={{ mb: 2 }}>
                <Grid2 size="auto">
                  <Button
                    component={Link}
                    to={routes.networkDevices}
                    variant="contained"
                    color="info"
                    disabled={isAnyOperationPending}
                  >
                    View Gateways
                  </Button>
                </Grid2>
                <Grid2>
                  <Typography variant="body2" color="text.secondary">
                    Manage and view all network gateways.
                  </Typography>
                </Grid2>
              </Grid2>

              <Grid2 container spacing={2} alignItems="center">
                <Grid2 size="auto">
                  <Button
                    variant="contained"
                    color="error"
                    onClick={deleteAllNetworkDevices}
                    startIcon={<DeleteForeverIcon />}
                    disabled={isAnyOperationPending}
                  >
                    Delete All Network Devices
                  </Button>
                </Grid2>
                <Grid2>
                  <Typography variant="body2" color="text.secondary">
                    <WarningIcon
                      sx={{
                        fontSize: 16,
                        verticalAlign: "text-bottom",
                        mr: 0.5,
                      }}
                    />
                    Warning! This will permanently delete all network devices.
                  </Typography>
                </Grid2>
              </Grid2>

              <Typography
                variant="body2"
                color="text.primary"
                sx={{ mt: 2, fontStyle: "italic" }}
              >
                Network devices are entities whose information is collected from
                data center routers.
              </Typography>
            </CardContent>
          </Card>
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
