import { ErrorResponseData } from "./ErrorResponseData";

export interface AxiosError extends Error {
  response?: {
    status: number;
    statusText: string;
    data: ErrorResponseData;
    headers: Record<string, string>;
    config?: unknown;
  };
  config?: unknown;
  code?: string;
  request?: unknown;
}
