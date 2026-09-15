import axios from "axios";
import {
  API_BASE_URL,
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
  AUTH_REGISTER_URL,
} from "@/config/api.config";
import { notifyUnauthorized } from "@/lib/auth/unauthorizedHandler";
import { uiStore } from "@/lib/stores/uiStore";
import { toApiError } from "./ApiError";

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30_000,
  // The session is an HttpOnly cookie (see authApi.cookieLogin), not a bearer
  // token attached here - this is what makes the browser send it on every
  // request and store the Set-Cookie from CookieLogin/RefreshToken in the
  // first place. The backend CORS policy pairs a concrete origin with
  // AllowCredentials() (see ServiceRegistration.cs) so this also works
  // cross-origin, e.g. `npm run dev` against a separately-hosted API.
  withCredentials: true,
});

const inlineAuthEndpoints = new Set([
  AUTH_REGISTER_URL,
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
]);

httpClient.interceptors.request.use(
  (config) => {
    uiStore.startRequest();
    return config;
  },
  (error) => Promise.reject(error)
);

httpClient.interceptors.response.use(
  (response) => {
    uiStore.endRequest();
    return response;
  },
  async (error: unknown) => {
    uiStore.endRequest();
    if (axios.isCancel(error)) return Promise.reject(error);

    const apiError = toApiError(error);
    const requestUrl = axios.isAxiosError(error)
      ? (error.config?.url ?? "")
      : "";
    const handlesErrorInline = [...inlineAuthEndpoints].some((endpoint) =>
      requestUrl.includes(endpoint)
    );

    if (apiError.status === 401 && !handlesErrorInline) {
      await notifyUnauthorized();
    }

    return Promise.reject(apiError);
  }
);

export default httpClient;
