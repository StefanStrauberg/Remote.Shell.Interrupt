import httpClient from "../httpClient";
import { DEFAULT_PAGINATION } from "../../constants/pagination";
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
export function parsePaginationHeader(header: unknown): PaginationMetadata {
  if (typeof header !== "string") {
    return { ...DEFAULT_PAGINATION };
  }

  try {
    const value: unknown = JSON.parse(header);

    if (
      typeof value !== "object" ||
      value === null ||
      !["TotalCount", "PageSize", "CurrentPage", "TotalPages"].every(
        (key) =>
          Number.isInteger((value as Record<string, unknown>)[key]) &&
          ((value as Record<string, number>)[key] ?? -1) >= 0
      ) ||
      typeof (value as Partial<PaginationMetadata>).HasNext !== "boolean" ||
      typeof (value as Partial<PaginationMetadata>).HasPrevious !== "boolean"
    ) {
      return { ...DEFAULT_PAGINATION };
    }

    return value as PaginationMetadata;
  } catch (parseError) {
    console.warn("Failed to parse pagination header:", parseError);
    return { ...DEFAULT_PAGINATION };
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
  const response = await httpClient.get<T[]>(url, { params });
  return {
    data: response.data,
    pagination: parsePaginationHeader(response.headers["x-pagination"]),
  };
}
