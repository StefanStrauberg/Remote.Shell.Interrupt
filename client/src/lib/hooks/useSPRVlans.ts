import { useQuery, UseQueryResult } from "@tanstack/react-query";
import { useLocation } from "react-router";
import { SprVlan } from "../types/SPRVlans/SprVlan";
import { PaginationMetadata } from "../types/Common/PaginationMetadata";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { defaultRetry, fetchPaged, PagedResponse } from "../api/common/paged";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";

interface UseSPRVlansReturn {
  sprVlans: SprVlan[];
  pagination: PaginationMetadata;
  isLoading: boolean;
  isError: boolean;
  error: unknown;
  refetch: () => void;
}

const SPR_VLANS_QUERY_KEY = "sprVlans";
const STALE_TIME_MS = 5 * 60 * 1000; // 5 minutes

export const useSPRVlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams
): UseSPRVlansReturn => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = [
    SPR_VLANS_QUERY_KEY,
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  async function fetchSprVlans(): Promise<PagedResponse<SprVlan>> {
    const params = buildRequestParams(pagination, orderBy, filters);
    return fetchPaged<SprVlan>("/api/SPRVlans/GetSPRVlansByFilter", params);
  }

  const {
    data: sprVlansResponse,
    isLoading,
    isError,
    error,
    refetch,
  }: UseQueryResult<PagedResponse<SprVlan>, unknown> = useQuery({
    queryKey,
    queryFn: fetchSprVlans,
    enabled: location.pathname === "/sprVlans",
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
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
