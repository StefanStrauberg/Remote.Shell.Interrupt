import { useState } from "react";
import { Link, Navigate, useLocation, useNavigate } from "react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { Login as LoginIcon, Storage } from "@mui/icons-material";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Typography
} from "@mui/material";
import TextInput from "../../../app/shared/components/TextInput";
import { loginSchema, LoginValues } from "../../../lib/schemas/AuthSchema";
import { useAuth } from "../../../lib/auth/useAuth";
import { getAuthErrorMessage } from "../../../lib/auth/authApi";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, loginAsync } = useAuth();
  const [submitError, setSubmitError] = useState<string | null>(null);

  // ProtectedRoute stores the originally requested path in location.state.
  const from = (location.state as { from?: string } | null)?.from ?? "/mainPage";

  const {
    control,
    handleSubmit,
    formState: { isValid }
  } = useForm<LoginValues>({
    mode: "onTouched",
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" }
  });

  const loginMutation = useMutation({
    mutationFn: async (values: LoginValues) => {
      await loginAsync(values.email, values.password);
    },
    onSuccess: () => navigate(from, { replace: true }),
    onError: (error) => setSubmitError(getAuthErrorMessage(error))
  });

  // Already signed in — no reason to show the form again.
  if (isAuthenticated) return <Navigate to="/mainPage" replace />;

  return (
    <Box
      sx={{
        minHeight: "90vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        px: 2
      }}
    >
      <Paper elevation={6} sx={{ p: 4, borderRadius: 3, width: "100%", maxWidth: 420 }}>
        <Box
          display="flex"
          flexDirection="column"
          alignItems="center"
          gap={1}
          mb={3}
        >
          <Storage color="primary" sx={{ fontSize: 48 }} />
          <Typography variant="h5" fontWeight="bold" component="h1">
            Sign in
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Remote Shell Interrupt platform
          </Typography>
        </Box>

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
          gap={3}
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
              loginMutation.isPending ? <CircularProgress size={20} /> : <LoginIcon />
            }
          >
            {loginMutation.isPending ? "Signing in..." : "Sign in"}
          </Button>

          <Button component={Link} to="/" color="inherit">
            Back to home
          </Button>
        </Box>
      </Paper>
    </Box>
  );
}
