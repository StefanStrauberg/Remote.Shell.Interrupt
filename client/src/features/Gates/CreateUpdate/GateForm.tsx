import { Alert, Box, Button, CircularProgress, Paper } from "@mui/material";
import {
  useCreateGateMutation,
  useGateQuery,
  useUpdateGateMutation,
} from "../api/gatesQueries";
import { Link, useParams, useNavigate } from "react-router";
import { gateSchema, GateSchema } from "../../../lib/schemas/GateSchema";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import SelectInput from "../../../app/shared/components/SelectInput";
import { typeOfNetworkDeviceOptions } from "../../../lib/types/Common/typeOfNetworkDeviceOptions";
import SaveIcon from "@mui/icons-material/Save";
import CancelIcon from "@mui/icons-material/Cancel";
import TextInput from "../../../app/shared/components/TextInput";
import { routes } from "../../../app/router/paths";
import RouterIcon from "@mui/icons-material/Router";
import PageHeader from "../../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../../app/shared/components/PageFeedback";

export default function GateForm() {
  const { id } = useParams();
  const navigate = useNavigate();

  const gateQuery = useGateQuery(id);
  const updateGate = useUpdateGateMutation();
  const createGate = useCreateGateMutation();
  const gate = gateQuery.data;

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
            navigate(routes.gates);
          },
        }
      );
    } else {
      createGate.mutate(data, {
        onSuccess: () => {
          navigate(routes.admin);
        },
      });
    }
  };

  useEffect(() => {
    if (gate) {
      reset(gate);
    }
  }, [gate, reset]);

  if (gateQuery.isLoading) {
    return <PageLoading message="Loading gate data…" />;
  }

  if (gateQuery.isError) {
    return (
      <Box>
        <PageError title="Unable to load gate" error={gateQuery.error} />
        <Button
          variant="outlined"
          component={Link}
          to={routes.gates}
          startIcon={<CancelIcon />}
          sx={{ mt: 2 }}
        >
          Back to gates
        </Button>
      </Box>
    );
  }

  const isSubmitting = updateGate.isPending || createGate.isPending;

  return (
    <Paper
      variant="outlined"
      sx={{ p: { xs: 2.5, sm: 4 }, maxWidth: 680, mx: "auto" }}
    >
      <PageHeader
        title={id ? "Edit gate" : "Create gate"}
        description="Configure the router identity and SNMP connection settings"
        icon={RouterIcon}
      />

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
          helperText={errors.name?.message}
          fullWidth
        />

        <TextInput
          label="IP Address"
          control={control}
          name="ipAddress"
          required
          helperText={errors.ipAddress?.message}
          placeholder="e.g., 192.168.1.1"
          fullWidth
        />

        <TextInput
          label="Community"
          control={control}
          name="community"
          required
          helperText={errors.community?.message}
          placeholder="SNMP community string"
          fullWidth
        />

        <SelectInput
          items={typeOfNetworkDeviceOptions}
          label="Device Type"
          control={control}
          name="typeOfNetworkDevice"
          required
          fullWidth
        />

        <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
          <Button
            variant="outlined"
            component={Link}
            to={id ? routes.gates : routes.admin}
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
