import { useQuery, UseQueryResult } from "@tanstack/react-query";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { SprVlan } from "../types/SPRVlans/SprVlan";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { SPRVlansResponse } from "../api/sprVlans/SPRVlansResponse";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { AxiosError } from "axios";

interface UseSPRVlansReturn {
  sprVlans: SprVlan[];
  pagination: typeof DEFAULT_PAGINATION;
  isLoading: boolean;
  isError: boolean;
  error: unknown;
  refetch: () => void;
}

// Type guard to check if error is an AxiosError
const isAxiosError = (error: unknown): error is AxiosError => {
  return error instanceof Error && "response" in error;
};

export const useSPRVlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams
): UseSPRVlansReturn => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = [
    "sprVlans",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  const {
    data: sprVlansResponse,
    isLoading,
    isError,
    error,
    refetch,
  }: UseQueryResult<SPRVlansResponse, unknown> = useQuery<SPRVlansResponse>({
    queryKey,
    queryFn: async (): Promise<SPRVlansResponse> => {
      try {
        const params = buildRequestParams(pagination, orderBy, filters);
        const response = await agent.get<SprVlan[]>(
          "/api/SPRVlans/GetSPRVlansByFilter",
          { params }
        );

        // Parse pagination metadata from headers
        const paginationHeader = response.headers["x-pagination"];
        let paginationData: typeof DEFAULT_PAGINATION = DEFAULT_PAGINATION;

        if (paginationHeader && typeof paginationHeader === "string") {
          try {
            paginationData = JSON.parse(paginationHeader);
          } catch (parseError) {
            console.warn("Failed to parse pagination header:", parseError);
          }
        }

        return {
          data: response.data,
          pagination: paginationData,
        };
      } catch (error) {
        console.error("Error fetching VLANs:", error);
        throw error;
      }
    },
    enabled: location.pathname === "/sprVlans",
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      if (isAxiosError(error) && error.response?.status === 404) return false;
      return failureCount < 3;
    },
  });

  return {
    sprVlans: sprVlansResponse?.data ?? [],
    pagination: sprVlansResponse?.pagination ?? DEFAULT_PAGINATION,
    isLoading,
    isError,
    error,
    refetch,
  };
};
