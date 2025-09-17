import {
  Box,
  Button,
  Paper,
  Typography,
  CircularProgress,
  Alert,
} from "@mui/material";
import { useGates } from "../../../lib/hooks/useGates";
import { Link, useParams, useNavigate } from "react-router";
import { gateSchema, GateSchema } from "../../../lib/schemas/GateSchema";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import SelectInput from "../../../app/shared/components/SelectInput";
import { typeOfNetworkDeviceOptions } from "../../../lib/types/Common/typeOfNetworkDeviceOptions";
import { DEFAULT_PAGINATION_PARAMS } from "../../../lib/types/Common/DEFAULT_PAGINATION_PARAMS";
import SaveIcon from "@mui/icons-material/Save";
import CancelIcon from "@mui/icons-material/Cancel";
import TextInput from "../../../app/shared/components/TextImput";

export default function GateForm() {
  const { id } = useParams();
  const navigate = useNavigate();

  const {
    updateGate,
    createGate,
    gate,
    isLoadingGate,
    isErrorGate,
    errorGate,
  } = useGates(
    DEFAULT_PAGINATION_PARAMS,
    [],
    { property: "", descending: false },
    id
  );

  const {
    control,
    reset,
    handleSubmit,
    formState: { errors, isDirty, isValid },
  } = useForm<GateSchema>({
    mode: "onTouched",
    resolver: zodResolver(gateSchema),
    defaultValues: {
      name: "",
      ipAddress: "",
      community: "",
      typeOfNetworkDevice: "",
    },
  });

  const onSubmit = (data: GateSchema) => {
    if (id) {
      updateGate.mutate(
        { ...data, id },
        {
          onSuccess: () => {
            navigate("/gates");
          },
        }
      );
    } else {
      createGate.mutate(data, {
        onSuccess: () => {
          navigate("/admin");
        },
      });
    }
  };

  useEffect(() => {
    if (gate) {
      reset(gate);
    }
  }, [gate, reset]);

  // Show loading state
  if (isLoadingGate) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="200px"
      >
        <CircularProgress />
        <Typography variant="h6" sx={{ ml: 2 }}>
          Loading gate data...
        </Typography>
      </Box>
    );
  }

  // Show error state
  if (isErrorGate) {
    return (
      <Paper sx={{ p: 3, borderRadius: 3 }}>
        <Alert severity="error" sx={{ mb: 2 }}>
          Error loading gate:{" "}
          {errorGate instanceof Error ? errorGate.message : "Unknown error"}
        </Alert>
        <Button
          variant="contained"
          component={Link}
          to="/gates"
          startIcon={<CancelIcon />}
        >
          Back to Gates
        </Button>
      </Paper>
    );
  }

  const isSubmitting = updateGate.isPending || createGate.isPending;

  return (
    <Paper
      sx={{ borderRadius: 3, padding: 3, maxWidth: 600, margin: "0 auto" }}
    >
      <Typography
        variant="h4"
        component="h1"
        gutterBottom
        color="primary"
        fontWeight="bold"
      >
        {id ? "Edit Gate" : "Create New Gate"}
      </Typography>

      {updateGate.isError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Error updating gate:{" "}
          {updateGate.error instanceof Error
            ? updateGate.error.message
            : "Unknown error"}
        </Alert>
      )}

      {createGate.isError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Error creating gate:{" "}
          {createGate.error instanceof Error
            ? createGate.error.message
            : "Unknown error"}
        </Alert>
      )}

      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        display="flex"
        flexDirection="column"
        gap={3}
      >
        <TextInput
          label="Name"
          control={control}
          name="name"
          required
          error={errors.name}
          helperText={errors.name?.message}
        />

        <TextInput
          label="IP Address"
          control={control}
          name="ipAddress"
          required
          error={errors.ipAddress}
          helperText={errors.ipAddress?.message}
          placeholder="e.g., 192.168.1.1"
        />

        <TextInput
          label="Community"
          control={control}
          name="community"
          required
          error={errors.community}
          helperText={errors.community?.message}
          placeholder="SNMP community string"
        />

        <SelectInput
          items={typeOfNetworkDeviceOptions}
          label="Device Type"
          control={control}
          name="typeOfNetworkDevice"
          required
          error={errors.typeOfNetworkDevice}
          helperText={errors.typeOfNetworkDevice?.message}
        />

        <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
          <Button
            variant="outlined"
            component={Link}
            to={id ? "/gates" : "/admin"}
            startIcon={<CancelIcon />}
            disabled={isSubmitting}
          >
            Cancel
          </Button>

          <Button
            type="submit"
            variant="contained"
            color="success"
            disabled={isSubmitting || !isDirty || !isValid}
            startIcon={
              isSubmitting ? <CircularProgress size={20} /> : <SaveIcon />
            }
          >
            {isSubmitting ? "Saving..." : "Save"}
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
