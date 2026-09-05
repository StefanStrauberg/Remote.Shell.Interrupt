import axios from "axios";
import agent from "../agent";
import { DEFAULT_PAGINATION } from "../../types/Common/DEFAULT_PAGINATION";
import { PaginationMetadata } from "../../types/Common/PaginationMetadata";

/**
 * A page of items plus the pagination metadata the API reports in the
 * `X-Pagination` response header.
 */
export type PagedResponse<T> = {
  data: T[];
  pagination: PaginationMetadata;
};

/**
 * Parses the `X-Pagination` response header. The backend serializes it
 * with a plain `JsonSerializer.Serialize` call, so the payload is
 * PascalCase (matching {@link PaginationMetadata}).
 */
function parsePaginationHeader(header: unknown): PaginationMetadata {
  if (typeof header !== "string") {
    return DEFAULT_PAGINATION;
  }

  try {
    return JSON.parse(header) as PaginationMetadata;
  } catch (parseError) {
    console.warn("Failed to parse pagination header:", parseError);
    return DEFAULT_PAGINATION;
  }
}

/**
 * Fetches a filtered page of items and combines the body with the
 * pagination metadata from the response header.
 */
export async function fetchPaged<T>(
  url: string,
  params?: Record<string, string | number | boolean>
): Promise<PagedResponse<T>> {
  const response = await agent.get<T[]>(url, { params });
  return {
    data: response.data,
    pagination: parsePaginationHeader(response.headers["x-pagination"]),
  };
}

/**
 * Shared TanStack Query retry policy: 404s are terminal (the axios
 * interceptor redirects to the not-found page), everything else is
 * retried up to three times.
 */
export function defaultRetry(failureCount: number, error: unknown): boolean {
  if (axios.isAxiosError(error) && error.response?.status === 404) return false;
  return failureCount < 3;
}
