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
  const token = useAuthStore((state) => state.token);
  const status = useAuthStore((state) => state.status);
  const queryClient = useQueryClient();

  const loginAsync = useCallback(
    async (email: string, password: string): Promise<AuthUser> => {
      const response = await authApi.login(email, password);
      const sessionUser: AuthUser = {
        id: response.userId,
        email: response.email,
        roles: response.roles
      };

      useAuthStore.getState().setSession(sessionUser, response.token);
      return sessionUser;
    },
    []
  );

  const cookieLoginAsync = useCallback(
    async (email: string, password: string, isPersistent = false): Promise<AuthUser> => {
      const response = await authApi.cookieLogin(email, password, isPersistent);
      const sessionUser: AuthUser = {
        id: response.userId,
        email: response.email,
        roles: response.roles
      };

      useAuthStore.getState().setSession(sessionUser, null);
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
    token,
    status,
    isAuthenticated: status === ("authenticated" satisfies AuthStatus),
    isAdmin: user?.roles.includes(ADMIN_ROLE) ?? false,
    loginAsync,
    cookieLoginAsync,
    registerAsync,
    logout
  };
}
