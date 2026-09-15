import axios from "axios";
import {
  API_BASE_URL,
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
  AUTH_LOGIN_URL,
  AUTH_REGISTER_URL,
} from "@/config/api.config";
import { useAuthStore } from "@/lib/auth/authStore";
import { notifyUnauthorized } from "@/lib/auth/unauthorizedHandler";
import { uiStore } from "@/lib/stores/uiStore";
import { toApiError } from "./ApiError";

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30_000,
});

const inlineAuthEndpoints = new Set([
  AUTH_LOGIN_URL,
  AUTH_REGISTER_URL,
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
]);

httpClient.interceptors.request.use(
  (config) => {
    uiStore.startRequest();
    const token = useAuthStore.getState().token;
    if (token) config.headers.set("Authorization", `Bearer ${token}`);
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
