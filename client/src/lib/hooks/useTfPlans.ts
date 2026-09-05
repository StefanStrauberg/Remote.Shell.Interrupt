import { useQuery, UseQueryResult } from "@tanstack/react-query";
import { TfPlan } from "../types/TfPlans/TfPlan";
import { useLocation } from "react-router";
import { PaginationMetadata } from "../types/Common/PaginationMetadata";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { PaginationParams } from "../types/Common/PaginationParams";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { defaultRetry, fetchPaged, PagedResponse } from "../api/common/paged";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";

interface UseTfPlansReturn {
  tfPlans: TfPlan[];
  pagination: PaginationMetadata;
  isLoading: boolean;
  isError: boolean;
  error: unknown;
  refetch: () => void;
}

const TF_PLANS_QUERY_KEY = "tfPlans";
const STALE_TIME_MS = 5 * 60 * 1000; // 5 minutes

export const useTfPlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams
): UseTfPlansReturn => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = [
    TF_PLANS_QUERY_KEY,
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  async function fetchTfPlans(): Promise<PagedResponse<TfPlan>> {
    const params = buildRequestParams(pagination, orderBy, filters);
    return fetchPaged<TfPlan>("/api/TfPlans/GetTfPlansByFilter", params);
  }

  const {
    data: tfPlansResponse,
    isLoading,
    isError,
    error,
    refetch,
  }: UseQueryResult<PagedResponse<TfPlan>, unknown> = useQuery({
    queryKey,
    queryFn: fetchTfPlans,
    enabled: location.pathname === "/tfPlans",
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
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
