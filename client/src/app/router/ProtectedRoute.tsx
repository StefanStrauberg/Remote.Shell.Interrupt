import { ReactNode } from "react";
import { Navigate, useLocation } from "react-router";
import { CircularProgress, Box } from "@mui/material";
import { hasAnyRole } from "../../lib/types/Auth/AuthUser";
import { useAuthStore } from "../../lib/auth/authStore";

type ProtectedRouteProps = {
  /** When set, the route is restricted to users holding at least one of these roles. */
  roles?: string[];
  children: ReactNode;
};

/**
 * Route guard: unauthenticated visitors are redirected to /login (the
 * original path travels in location.state so the login page can return
 * them), authenticated users missing a required role are sent to
 * /access-denied. Wrap any route element:
 *
 *   <ProtectedRoute roles={["Admin"]}><GatesDashboard /></ProtectedRoute>
 */
export default function ProtectedRoute({ roles, children }: ProtectedRouteProps) {
  const location = useLocation();
  const user = useAuthStore((state) => state.user);
  const status = useAuthStore((state) => state.status);

  if (status === "idle") {
    // Defensive: restoreSession() runs synchronously before the first render.
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (status !== "authenticated") {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (!hasAnyRole(user, roles)) {
    return <Navigate to="/access-denied" replace state={{ from: location.pathname }} />;
  }

  return <>{children}</>;
}
