/**
 * Pure API endpoint configuration.
 *
 * This module MUST stay import-free: it is consumed by the axios client
 * (`lib/api/agent.ts`), the auth module (`lib/auth/authApi.ts`) and — through
 * them — by the router, pages and hooks. Keeping zero internal imports
 * guarantees the constants are fully evaluated before any consumer's module
 * body runs, which makes circular-dependency initialization errors
 * ("Cannot access 'AUTH_LOGIN_URL' before initialization") impossible.
 */

/** Backend base URL, injected at build time from the environment. */
export const API_BASE_URL = import.meta.env.VITE_API_URL;

export const AUTH_LOGIN_URL = "/api/Auth/Login";
export const AUTH_REGISTER_URL = "/api/Auth/Register";
export const AUTH_COOKIE_LOGIN_URL = "/api/Auth/CookieLogin";
export const AUTH_COOKIE_LOGOUT_URL = "/api/Auth/CookieLogout";
