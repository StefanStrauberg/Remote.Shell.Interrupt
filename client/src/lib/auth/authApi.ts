import httpClient from "../api/httpClient";
import { ApiError } from "../api/ApiError";
import {
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
  AUTH_REGISTER_URL,
} from "../../config/api.config";

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
   * Establishes an HttpOnly cookie session — the SPA's only authentication
   * flow, so no bearer token ever exists in JS-accessible storage.
   * withCredentials is on by default for every request (see httpClient.ts);
   * the backend CORS policy pairs a concrete origin with AllowCredentials()
   * so the browser actually accepts and stores the cross-origin cookie.
   */
  async cookieLogin(
    email: string,
    password: string,
    isPersistent: boolean
  ): Promise<AuthCookieLoginResponse> {
    const response = await httpClient.post<AuthCookieLoginResponse>(
      AUTH_COOKIE_LOGIN_URL,
      { email, password, isPersistent }
    );
    return response.data;
  },

  async cookieLogout(): Promise<void> {
    await httpClient.post(AUTH_COOKIE_LOGOUT_URL, {});
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
