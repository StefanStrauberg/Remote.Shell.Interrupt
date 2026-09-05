import axios from "axios";
import { store } from "../stores/store";
import { toast } from "react-toastify";
import { router } from "../../app/router/Routes";

const agent = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

agent.interceptors.request.use((config) => {
  store.uiStore.isBusy();
  return config;
});

agent.interceptors.response.use(
  async (response) => {
    store.uiStore.isIdle();
    return response;
  },
  async (error) => {
    store.uiStore.isIdle();
    const { status, data } = error.response;
    switch (status) {
      case 422:
        if (data.Errors) {
          const modalStateErrors = [];
          for (const key in data.Errors) {
            if (data.Errors[key]) {
              modalStateErrors.push(data.Errors[key]);
            }
          }
          throw modalStateErrors.flat();
        } else {
          toast.error(data.Title);
        }
        break;
      case 400:
        toast.error(data.Detail);
        break;
      case 401:
        toast.error("Unauthorised");
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
