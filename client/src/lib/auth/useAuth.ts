import { useCallback } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { ADMIN_ROLE } from "../types/Auth/AuthUser";
import { AuthUser } from "../types/Auth/AuthUser";
import { AuthStatus, useAuthStore } from "./authStore";
import { authApi } from "./authApi";

/**
 * Facade over the auth store: exposes the session, derived flags and the
 * lifecycle actions used by pages, the navigation bar and route guards.
 */
export function useAuth() {
  const user = useAuthStore((state) => state.user);
  const status = useAuthStore((state) => state.status);
  const queryClient = useQueryClient();

  const loginAsync = useCallback(
    async (
      email: string,
      password: string,
      isPersistent = true
    ): Promise<AuthUser> => {
      const response = await authApi.cookieLogin(email, password, isPersistent);

      if (
        !response.userId ||
        !response.email ||
        !Array.isArray(response.roles)
      ) {
        throw new Error("The server returned an invalid login response.");
      }
      const sessionUser: AuthUser = {
        id: response.userId,
        email: response.email,
        roles: response.roles,
      };

      useAuthStore.getState().setSession(sessionUser);
      return sessionUser;
    },
    []
  );

  const registerAsync = useCallback(
    async (email: string, password: string, role: string) =>
      authApi.register(email, password, role),
    []
  );

  const logout = useCallback(async (): Promise<void> => {
    // Best effort: the cookie session may already be gone server-side.
    try {
      await authApi.cookieLogout();
    } catch {
      /* ignore */
    }
    useAuthStore.getState().clearSession();
    // Drop every cached business payload belonging to the closed session.
    queryClient.clear();
  }, [queryClient]);

  return {
    user,
    status,
    isAuthenticated: status === ("authenticated" satisfies AuthStatus),
    isAdmin: user?.roles.includes(ADMIN_ROLE) ?? false,
    loginAsync,
    registerAsync,
    logout,
  };
}
