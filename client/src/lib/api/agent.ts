import axios from "axios";
import { toast } from "react-toastify";
import { uiStore } from "../stores/uiStore";
import { useAuthStore } from "../auth/authStore";
import { queryClient } from "../queryClient";
import { ApiErrorResponse } from "../types/Common/ApiErrorResponse";
import {
  API_BASE_URL,
  AUTH_COOKIE_LOGIN_URL,
  AUTH_LOGIN_URL
} from "../../config/api.config";

const agent = axios.create({
  baseURL: API_BASE_URL
});

agent.interceptors.request.use((config) => {
  uiStore.startRequest();

  // Attach the JWT to every request when a session exists.
  const token = useAuthStore.getState().token;

  if (token) {
    config.headers.set("Authorization", `Bearer ${token}`);
  }

  return config;
});

/** Auth endpoints manage their own 4xx feedback inline (login/register pages). */
const AUTH_ENDPOINTS_WITH_INLINE_ERROR_HANDLING: string[] = [
  AUTH_LOGIN_URL,
  AUTH_COOKIE_LOGIN_URL
];

/**
 * Resolves the router lazily via dynamic import.
 *
 * A static `import { router } from "app/router/Routes"` here would recreate
 * the circular dependency Routes → layout → auth module → agent → Routes and
 * crash startup with "Cannot access ... before initialization". The dynamic
 * edge keeps the static module graph acyclic; the router is fully evaluated
 * by the time any response interceptor can run.
 */
async function getRouter() {
  const { router } = await import("../../app/router/Routes");
  return router;
}

agent.interceptors.response.use(
  async (response) => {
    uiStore.endRequest();
    return response;
  },
  async (error) => {
    uiStore.endRequest();

    // Network failure, CORS rejection or aborted request: no response at all.
    if (!error.response) {
      toast.error("Network error. Please check your connection and try again.");
      return Promise.reject(error);
    }

    const status: number = error.response.status;
    const data = error.response.data as Partial<ApiErrorResponse> & { error?: string };
    const requestUrl: string = error.config?.url ?? "";
    const isAuthRequest = AUTH_ENDPOINTS_WITH_INLINE_ERROR_HANDLING.some((url) =>
      requestUrl.includes(url)
    );

    switch (status) {
      case 422: {
        if (data.Errors) {
          const validationErrors = Object.values(data.Errors).flat();
          // Thrown as an array of messages; consumers (e.g. TestErrors)
          // surface each entry through their onError handler.
          throw validationErrors;
        }
        toast.error(data.Title);
        break;
      }
      case 400:
        // Auth forms display the message inline — no global toast here.
        if (!isAuthRequest) {
          toast.error(data.Detail ?? data.error ?? "Bad request.");
        }
        break;
      case 401: {
        // Invalid login attempts are handled by the auth forms themselves.
        if (isAuthRequest) break;

        // Expired/revoked session anywhere in the app: tear the session down
        // and boot the user back to the login screen.
        useAuthStore.getState().clearSession();
        queryClient.clear();

        const router = await getRouter();

        if (router.state.location.pathname !== "/login") {
          toast.error("Your session has expired. Please sign in again.");
          await router.navigate("/login");
        }
        break;
      }
      case 403:
        toast.error("You do not have permission to perform this action.");
        break;
      case 404:
        (await getRouter()).navigate("/not-found");
        break;
      case 500:
        (await getRouter()).navigate("/server-error", { state: { error: data } });
        break;
      default:
        break;
    }

    return Promise.reject(error);
  }
);

export default agent;
