import { env } from "./env";

/** Backend base URL, injected at build time from the environment. */
export const API_BASE_URL = env.apiBaseUrl;

/** Route prefix declared by BaseAPIController in the backend. */
export const API_V1_PREFIX = "/api/v1";

export function apiPath(...segments: Array<string | number>): string {
  const suffix = segments.map((segment) => encodeURIComponent(String(segment)));
  return [API_V1_PREFIX, ...suffix].join("/");
}

export const AUTH_LOGIN_URL = `${API_V1_PREFIX}/Auth/Login`;
export const AUTH_REGISTER_URL = `${API_V1_PREFIX}/Auth/Register`;
export const AUTH_COOKIE_LOGIN_URL = `${API_V1_PREFIX}/Auth/CookieLogin`;
export const AUTH_COOKIE_LOGOUT_URL = `${API_V1_PREFIX}/Auth/CookieLogout`;
