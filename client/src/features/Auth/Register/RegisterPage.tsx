import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { PersonAddAlt as PersonAddAltIcon } from "@mui/icons-material";
import { Alert, Box, Button, CircularProgress } from "@mui/material";
import { toast } from "react-toastify";
import TextInput from "../../../app/shared/components/TextInput";
import SelectInput from "../../../app/shared/components/SelectInput";
import {
  registerSchema,
  RegisterValues,
} from "../../../lib/schemas/AuthSchema";
import { useAuth } from "../../../lib/auth/useAuth";
import { getAuthErrorMessage } from "../../../lib/auth/authApi";
import { authRoles } from "../../../lib/schemas/AuthSchema";
import { routes } from "../../../app/router/paths";
import { userKeys } from "../../Users/api/usersQueries";
import AuthLayout from "../components/AuthLayout";

const roleOptions = authRoles.map((role) => ({
  text: role === "Admin" ? "Administrator" : "User",
  value: role,
}));

export default function RegisterPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { isAuthenticated, isAdmin, registerAsync } = useAuth();
  const [submitError, setSubmitError] = useState<string | null>(null);

  const {
    control,
    handleSubmit,
    formState: { isValid },
  } = useForm<RegisterValues>({
    mode: "onTouched",
    resolver: zodResolver(registerSchema),
    defaultValues: {
      email: "",
      password: "",
      confirmPassword: "",
      role: "User",
    },
  });

  const registerMutation = useMutation({
    mutationFn: async (values: RegisterValues) => {
      const result = await registerAsync(
        values.email,
        values.password,
        values.role
      );

      if (!result.success) {
        throw new Error(result.error ?? "Registration failed.");
      }

      return result;
    },
    onSuccess: () => {
      toast.success("Account created successfully.");
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      navigate(routes.adminUsers);
    },
    onError: (error) => setSubmitError(getAuthErrorMessage(error)),
  });

  // Registration provisions accounts — administrators only, matching the API.
  if (!isAuthenticated) return <Navigate to={routes.login} replace />;
  if (!isAdmin) return <Navigate to={routes.accessDenied} replace />;

  return (
    <AuthLayout
      title="Create account"
      subtitle="Administrator provisioning — new accounts can sign in immediately"
      maxWidth={480}
    >
      {submitError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {submitError}
        </Alert>
      )}

      <Box
        component="form"
        onSubmit={handleSubmit((values) => {
          setSubmitError(null);
          registerMutation.mutate(values);
        })}
        display="flex"
        flexDirection="column"
        gap={2.5}
        noValidate
      >
        <TextInput
          label="Email"
          control={control}
          name="email"
          type="email"
          required
          fullWidth
          autoComplete="email"
        />

        <TextInput
          label="Password"
          control={control}
          name="password"
          type="password"
          showPasswordToggle
          required
          fullWidth
          autoComplete="new-password"
          tooltip="At least 10 characters with upper/lower case letters, a digit and a special character"
        />

        <TextInput
          label="Confirm password"
          control={control}
          name="confirmPassword"
          type="password"
          showPasswordToggle
          required
          fullWidth
          autoComplete="new-password"
        />

        <SelectInput
          items={roleOptions}
          label="Role"
          control={control}
          name="role"
          required
          fullWidth
        />

        <Button
          type="submit"
          variant="contained"
          color="success"
          disabled={registerMutation.isPending || !isValid}
          startIcon={
            registerMutation.isPending ? (
              <CircularProgress size={20} />
            ) : (
              <PersonAddAltIcon />
            )
          }
        >
          {registerMutation.isPending ? "Creating..." : "Create account"}
        </Button>

        <Button component={Link} to={routes.admin} color="inherit">
          Back to admin panel
        </Button>
      </Box>
    </AuthLayout>
  );
}
