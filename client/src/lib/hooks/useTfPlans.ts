import { useQuery, UseQueryResult } from "@tanstack/react-query";
import { TfPlan } from "../types/TfPlans/TfPlan";
import agent from "../api/agent";
import { useLocation } from "react-router";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { PaginationParams } from "../types/Common/PaginationParams";
import { TfPlansResponse } from "../api/tfPlans/TfPlansResponse";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { AxiosError } from "axios";

interface UseTfPlansReturn {
  tfPlans: TfPlan[];
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

export const useTfPlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams
): UseTfPlansReturn => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = [
    "tfPlans",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  const {
    data: tfPlansResponse,
    isLoading,
    isError,
    error,
    refetch,
  }: UseQueryResult<TfPlansResponse, unknown> = useQuery<TfPlansResponse>({
    queryKey,
    queryFn: async (): Promise<TfPlansResponse> => {
      try {
        const params = buildRequestParams(pagination, orderBy, filters);
        const response = await agent.get<TfPlan[]>(
          "/api/TfPlans/GetTfPlansByFilter",
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
        console.error("Error fetching tariff plans:", error);
        throw error;
      }
    },
    enabled: location.pathname === "/tfPlans",
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      // Don't retry on 404 errors
      if (isAxiosError(error) && error.response?.status === 404) return false;
      // Retry up to 3 times for other errors
      return failureCount < 3;
    },
  });

  return {
    tfPlans: tfPlansResponse?.data ?? [],
    pagination: tfPlansResponse?.pagination ?? DEFAULT_PAGINATION,
    isLoading,
    isError,
    error,
    refetch,
  };
};
