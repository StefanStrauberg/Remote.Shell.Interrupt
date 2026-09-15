import axios from "axios";
import { ApiError } from "./ApiError";

/** Retries only transient failures for which no HTTP response was received. */
export function shouldRetryQuery(
  failureCount: number,
  error: unknown
): boolean {
  const isNetworkError =
    (error instanceof ApiError && error.isNetworkError) ||
    (axios.isAxiosError(error) && !error.response);
  return isNetworkError && failureCount < 2;
}
