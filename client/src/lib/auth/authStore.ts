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

function getStoredValue(key: string): string | null {
  try {
    return localStorage.getItem(key);
  } catch {
    return null;
  }
}

function setStoredValue(key: string, value: string): void {
  try {
    localStorage.setItem(key, value);
  } catch {
    // The in-memory session still works when storage is unavailable.
  }
}

function removeStoredValue(key: string): void {
  try {
    localStorage.removeItem(key);
  } catch {
    // Storage may be blocked by the browser or an embedding policy.
  }
}

function readStoredUser(): AuthUser | null {
  try {
    const raw = getStoredValue(USER_STORAGE_KEY);
    if (!raw) return null;

    const value: unknown = JSON.parse(raw);
    if (
      typeof value !== "object" ||
      value === null ||
      typeof (value as Partial<AuthUser>).id !== "string" ||
      typeof (value as Partial<AuthUser>).email !== "string" ||
      !Array.isArray((value as Partial<AuthUser>).roles) ||
      !(value as Partial<AuthUser>).roles?.every(
        (role) => typeof role === "string"
      )
    ) {
      return null;
    }

    return value as AuthUser;
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
    roles: rolesFromJwtPayload(payload),
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
      setStoredValue(TOKEN_STORAGE_KEY, token);
    } else {
      // Cookie session: any stale token must not be reused.
      removeStoredValue(TOKEN_STORAGE_KEY);
    }
    setStoredValue(USER_STORAGE_KEY, JSON.stringify(user));
    set({ user, token, status: "authenticated" });
  },

  clearSession: () => {
    removeStoredValue(TOKEN_STORAGE_KEY);
    removeStoredValue(USER_STORAGE_KEY);
    set({ user: null, token: null, status: "unauthenticated" });
  },

  /**
   * Silent session check performed once on application load: validates the
   * persisted token's expiry locally (the axios 401 interceptor covers any
   * server-side revocation) and restores the profile, so page refreshes
   * keep the user signed in.
   */
  restoreSession: () => {
    const token = getStoredValue(TOKEN_STORAGE_KEY);
    const storedUser = readStoredUser();

    // 1) JWT present but expired -> drop everything.
    if (token && isTokenExpired(token, TOKEN_EXPIRY_LEEWAY_SECONDS)) {
      removeStoredValue(TOKEN_STORAGE_KEY);
      removeStoredValue(USER_STORAGE_KEY);
      set({ user: null, token: null, status: "unauthenticated" });
      return;
    }

    // 2) Valid JWT -> rebuild the profile from its claims. Persisted profile
    // data is only a cache and must not override roles carried by the token.
    if (token) {
      const user = userFromToken(token);

      if (user) {
        setStoredValue(USER_STORAGE_KEY, JSON.stringify(user));
        set({ user, token, status: "authenticated" });
        return;
      }

      removeStoredValue(TOKEN_STORAGE_KEY);
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
  },
}));
