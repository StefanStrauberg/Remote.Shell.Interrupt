import { create } from "zustand";
import { AuthUser } from "../types/Auth/AuthUser";
import { decodeJwtPayload, isTokenExpired, rolesFromJwtPayload } from "./jwt";

const TOKEN_STORAGE_KEY = "rsi.auth.token";
const USER_STORAGE_KEY = "rsi.auth.user";
const TOKEN_EXPIRY_LEEWAY_SECONDS = 30;

export type AuthStatus = "idle" | "authenticated" | "unauthenticated";

interface AuthState {
  /** Current user profile (null when signed out). */
  user: AuthUser | null;
  /** JWT access token (null for HttpOnly-cookie sessions). */
  token: string | null;
  /** "idle" before bootstrapping, then "authenticated" | "unauthenticated". */
  status: AuthStatus;

  setSession: (user: AuthUser, token: string | null) => void;
  clearSession: () => void;
  restoreSession: () => void;
}

function readStoredUser(): AuthUser | null {
  try {
    const raw = localStorage.getItem(USER_STORAGE_KEY);
    return raw ? (JSON.parse(raw) as AuthUser) : null;
  } catch {
    return null;
  }
}

/** Rebuilds a minimal profile from token claims when storage was cleared. */
function userFromToken(token: string): AuthUser | null {
  const payload = decodeJwtPayload(token);

  if (!payload?.sub) return null;

  return {
    id: payload.sub,
    email: payload.email ?? "",
    roles: rolesFromJwtPayload(payload)
  };
}

/**
 * Global authentication state.
 *
 * Deliberately framework-independent (zustand, not React Context) so the
 * axios interceptor can read/clear the session from outside the component
 * tree. The JWT is kept in localStorage because the backend issues it in
 * the login response body; switch to the cookie flow for HttpOnly storage.
 */
export const useAuthStore = create<AuthState>()((set) => ({
  user: null,
  token: null,
  status: "idle",

  setSession: (user, token) => {
    if (token) {
      localStorage.setItem(TOKEN_STORAGE_KEY, token);
    } else {
      // Cookie session: any stale token must not be reused.
      localStorage.removeItem(TOKEN_STORAGE_KEY);
    }
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
    set({ user, token, status: "authenticated" });
  },

  clearSession: () => {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    localStorage.removeItem(USER_STORAGE_KEY);
    set({ user: null, token: null, status: "unauthenticated" });
  },

  /**
   * Silent session check performed once on application load: validates the
   * persisted token's expiry locally (the axios 401 interceptor covers any
   * server-side revocation) and restores the profile, so page refreshes
   * keep the user signed in.
   */
  restoreSession: () => {
    const token = localStorage.getItem(TOKEN_STORAGE_KEY);
    const storedUser = readStoredUser();

    // 1) JWT present but expired -> drop everything.
    if (token && isTokenExpired(token, TOKEN_EXPIRY_LEEWAY_SECONDS)) {
      localStorage.removeItem(TOKEN_STORAGE_KEY);
      localStorage.removeItem(USER_STORAGE_KEY);
      set({ user: null, token: null, status: "unauthenticated" });
      return;
    }

    // 2) Valid JWT -> restore (rebuild the profile from claims if needed).
    if (token) {
      const user = storedUser ?? userFromToken(token);

      if (user) {
        localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
        set({ user, token, status: "authenticated" });
        return;
      }

      localStorage.removeItem(TOKEN_STORAGE_KEY);
      set({ user: null, token: null, status: "unauthenticated" });
      return;
    }

    // 3) No JWT: an HttpOnly cookie session may still be alive. Restore the
    // profile optimistically; the 401 interceptor boots the user if the
    // server has expired the cookie.
    if (storedUser) {
      set({ user: storedUser, token: null, status: "authenticated" });
      return;
    }

    set({ user: null, token: null, status: "unauthenticated" });
  }
}));
