import { create } from "zustand";
import { AuthUser } from "../types/Auth/AuthUser";

const USER_STORAGE_KEY = "rsi.auth.user";

export type AuthStatus = "idle" | "authenticated" | "unauthenticated";

interface AuthState {
  /** Current user profile (null when signed out). */
  user: AuthUser | null;
  /** "idle" before bootstrapping, then "authenticated" | "unauthenticated". */
  status: AuthStatus;

  setSession: (user: AuthUser) => void;
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

/**
 * Global authentication state.
 *
 * Deliberately framework-independent (zustand, not React Context) so the
 * axios interceptor can read/clear the session from outside the component
 * tree. The session itself is an HttpOnly cookie the backend sets on
 * CookieLogin (see authApi.cookieLogin) - inaccessible to this code by
 * design, which is the whole point: nothing here ever holds a bearer token
 * that a successful XSS could read out of storage. What's cached is just the
 * non-secret profile (id/email/roles) for a smoother reload; it carries no
 * authentication power on its own, so it isn't a bearer credential to leak.
 * A page refresh restores this profile optimistically and the axios 401
 * interceptor (see unauthorizedHandler) boots the user if the server has
 * actually expired the cookie.
 */
export const useAuthStore = create<AuthState>()((set) => ({
  user: null,
  status: "idle",

  setSession: (user) => {
    setStoredValue(USER_STORAGE_KEY, JSON.stringify(user));
    set({ user, status: "authenticated" });
  },

  clearSession: () => {
    removeStoredValue(USER_STORAGE_KEY);
    set({ user: null, status: "unauthenticated" });
  },

  restoreSession: () => {
    const storedUser = readStoredUser();

    if (storedUser) {
      set({ user: storedUser, status: "authenticated" });
      return;
    }

    set({ user: null, status: "unauthenticated" });
  },
}));
