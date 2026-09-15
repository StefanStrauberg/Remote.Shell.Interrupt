import { useState } from "react";
import { Navigate, useLocation, useNavigate } from "react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { Login as LoginIcon } from "@mui/icons-material";
import { Alert, Box, Button, CircularProgress } from "@mui/material";
import TextInput from "../../../app/shared/components/TextInput";
import { loginSchema, LoginValues } from "../../../lib/schemas/AuthSchema";
import { useAuth } from "../../../lib/auth/useAuth";
import { getAuthErrorMessage } from "../../../lib/auth/authApi";
import { routes } from "../../../app/router/paths";
import AuthLayout from "../components/AuthLayout";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, loginAsync } = useAuth();
  const [submitError, setSubmitError] = useState<string | null>(null);

  // ProtectedRoute stores the originally requested path in location.state.
  const from =
    (location.state as { from?: string } | null)?.from ?? routes.main;

  const {
    control,
    handleSubmit,
    formState: { isValid },
  } = useForm<LoginValues>({
    mode: "onTouched",
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  const loginMutation = useMutation({
    mutationFn: async (values: LoginValues) => {
      await loginAsync(values.email, values.password);
    },
    onSuccess: () => navigate(from, { replace: true }),
    onError: (error) => setSubmitError(getAuthErrorMessage(error)),
  });

  // Already signed in — no reason to show the form again.
  if (isAuthenticated) return <Navigate to={routes.main} replace />;

  return (
    <AuthLayout title="Sign in" subtitle="Remote Shell Interrupt platform">
      {submitError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {submitError}
        </Alert>
      )}

      <Box
        component="form"
        onSubmit={handleSubmit((values) => {
          setSubmitError(null);
          loginMutation.mutate(values);
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
          autoComplete="current-password"
        />

        <Button
          type="submit"
          variant="contained"
          size="large"
          disabled={loginMutation.isPending || !isValid}
          startIcon={
            loginMutation.isPending ? (
              <CircularProgress size={20} />
            ) : (
              <LoginIcon />
            )
          }
        >
          {loginMutation.isPending ? "Signing in..." : "Sign in"}
        </Button>
      </Box>
    </AuthLayout>
  );
}
