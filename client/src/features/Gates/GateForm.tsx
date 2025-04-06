import { Box, Button, ButtonGroup, Paper, Typography } from "@mui/material";
import { useGates } from "../../lib/hooks/useGates";
import { Link, useParams, useNavigate } from "react-router";
import { gateSchema, GateSchema } from "../../lib/schemas/GateSchema";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import TextInput from "../../app/shared/components/TextImput";
import SelectInput from "../../app/shared/components/SelectInput";
import { typeOfNetworkDeviceOptions } from "../../lib/types/typeOfNetworkDeviceOptions";

export default function GateForm() {
  const { id } = useParams();
  const navigate = useNavigate(); // Initialize useNavigate hook
  const { updateGate, createGate, gate, isLoadingGate } = useGates(
    0,
    0,
    {},
    id
  );

  const { control, reset, handleSubmit } = useForm<GateSchema>({
    mode: "onTouched",
    resolver: zodResolver(gateSchema),
  });

  const onSubmit = (data: GateSchema) => {
    if (id) {
      updateGate.mutate(
        { ...data, id },
        {
          onSuccess: () => {
            navigate("/gates");
          },
          onError: (error) => {
            console.log(error);
          },
        }
      );
    } else {
      createGate.mutate(data, {
        onSuccess: () => {
          navigate("/gates");
        },
        onError: (error) => {
          console.log(error);
        },
      });
    }
  };

  useEffect(() => {
    if (gate) reset(gate);
  }, [gate, reset]);

  if (isLoadingGate) return <Typography>Loading activity...</Typography>;

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        {gate ? "Edit gate" : "Create gate"}
      </Typography>
      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        display="flex"
        flexDirection="column"
        gap={3}
      >
        <TextInput label="Name" control={control} name="name" />
        <TextInput label="IPAddress" control={control} name="ipAddress" />
        <TextInput label="Community" control={control} name="community" />
        <SelectInput
          items={typeOfNetworkDeviceOptions}
          label="Type Of NetworkDevice"
          control={control}
          name="typeOfNetworkDevice"
        />
        <Box display="flex" justifyContent="end" gap={3}>
          <ButtonGroup variant="contained">
            <Button color="primary" component={Link} to={`/gates`}>
              Отмена
            </Button>
            <Button
              type="submit"
              color="success"
              disabled={updateGate.isPending || createGate.isPending}
            >
              Сохранить
            </Button>
          </ButtonGroup>
        </Box>
      </Box>
    </Paper>
  );
}
