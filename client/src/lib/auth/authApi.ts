import httpClient from "../api/httpClient";
import { ApiError } from "../api/ApiError";
import {
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
  AUTH_LOGIN_URL,
  AUTH_REGISTER_URL,
} from "../../config/api.config";

/** Body of POST /api/Auth/Login — serialized camelCase by MVC. */
export type AuthLoginResponse = {
  success: boolean;
  token: string | null;
  expiresAtUtc?: string;
  userId: string | null;
  email: string | null;
  roles: string[];
  error?: string | null;
};

/** Body of POST /api/Auth/Register. */
export type AuthRegisterResponse = {
  success: boolean;
  userId: string | null;
  error?: string;
};

/** Body of POST /api/Auth/CookieLogin (no token — the session is the cookie). */
export type AuthCookieLoginResponse = {
  userId: string;
  email: string;
  roles: string[];
};

export const authApi = {
  async login(email: string, password: string): Promise<AuthLoginResponse> {
    const response = await httpClient.post<AuthLoginResponse>(AUTH_LOGIN_URL, {
      email,
      password,
    });
    return response.data;
  },

  async register(
    email: string,
    password: string,
    role: string
  ): Promise<AuthRegisterResponse> {
    const response = await httpClient.post<AuthRegisterResponse>(
      AUTH_REGISTER_URL,
      {
        email,
        password,
        role,
      }
    );
    return response.data;
  },

  /**
   * Establishes an HttpOnly cookie session. Requires the backend CORS policy
   * to allow the frontend origin with credentials (AllowCredentials + explicit
   * origins) — withCredentials is set per-request so the JWT flow keeps
   * working against the current wildcard CORS policy.
   */
  async cookieLogin(
    email: string,
    password: string,
    isPersistent: boolean
  ): Promise<AuthCookieLoginResponse> {
    const response = await httpClient.post<AuthCookieLoginResponse>(
      AUTH_COOKIE_LOGIN_URL,
      { email, password, isPersistent },
      { withCredentials: true }
    );
    return response.data;
  },

  async cookieLogout(): Promise<void> {
    await httpClient.post(
      AUTH_COOKIE_LOGOUT_URL,
      {},
      { withCredentials: true }
    );
  },
};

/** Maps any auth-flow failure to a user-facing message. */
export function getAuthErrorMessage(error: unknown): string {
  // The axios interceptor throws an array of messages for 422 responses.
  if (Array.isArray(error)) {
    return error.map(String).join("; ");
  }

  if (error instanceof ApiError) {
    switch (error.status) {
      case 401:
        return "Invalid email or password.";
      case 403:
        return "Your account is not allowed to perform this action.";
      default:
        return error.message;
    }
  }

  if (error instanceof Error && error.message) {
    return error.message;
  }

  return "Authentication failed. Please try again.";
}
