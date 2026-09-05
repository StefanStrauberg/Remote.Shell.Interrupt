import axios from "axios";
import { toast } from "react-toastify";
import { router } from "../../app/router/Routes";
import { uiStore } from "../stores/uiStore";
import { ApiErrorResponse } from "../types/Common/ApiErrorResponse";

const agent = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

agent.interceptors.request.use((config) => {
  uiStore.startRequest();
  return config;
});

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

    const { status, data } = error.response as {
      status: number;
      data: ApiErrorResponse;
    };

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
        toast.error(data.Detail);
        break;
      case 401:
        toast.error("Unauthorized");
        break;
      case 404:
        router.navigate("/not-found");
        break;
      case 500:
        router.navigate("/server-error", { state: { error: data } });
        break;
      default:
        break;
    }

    return Promise.reject(error);
  }
);

export default agent;
