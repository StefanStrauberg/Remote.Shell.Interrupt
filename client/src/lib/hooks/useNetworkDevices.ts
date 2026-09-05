import {
  useMutation,
  useQuery,
  useQueryClient,
  UseMutationResult,
  UseQueryResult,
} from "@tanstack/react-query";
import { NetworkDevice } from "../types/NetworkDevices/NetworkDevice";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { PaginationMetadata } from "../types/Common/PaginationMetadata";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { defaultRetry, fetchPaged, PagedResponse } from "../api/common/paged";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { DEFAULT_PAGINATION_PARAMS } from "../types/Common/DEFAULT_PAGINATION_PARAMS";

interface UseNetworkDevicesReturn {
  networkDevices: NetworkDevice[];
  isPending: boolean;
  isError: boolean;
  error: unknown;
  pagination: PaginationMetadata;
  deleteNetworkDevices: UseMutationResult<void, Error, void, unknown>;
  networkDevice: NetworkDevice | undefined;
  isLoadingNetworkDevice: boolean;
  isErrorNetworkDevice: boolean;
  errorNetworkDevice: unknown;
  refetchNetworkDevices: () => void;
  refetchNetworkDevice: () => void;
}

const NETWORK_DEVICES_QUERY_KEY = "networkDevices";
const STALE_TIME_MS = 5 * 60 * 1000; // 5 minutes

export const useNetworkDevices = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams,
  id?: string
): UseNetworkDevicesReturn => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;
  const queryClient = useQueryClient();

  const queryKey = [
    NETWORK_DEVICES_QUERY_KEY,
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  // Query for network devices list
  async function fetchNetworkDevices(): Promise<PagedResponse<NetworkDevice>> {
    const params = buildRequestParams(pagination, orderBy, filters);
    return fetchPaged<NetworkDevice>(
      "/api/NetworkDevices/GetNetworkDevicesByFilter",
      params
    );
  }

  const {
    data: networkDevicesResponse,
    isPending,
    isError,
    error,
    refetch: refetchNetworkDevices,
  }: UseQueryResult<PagedResponse<NetworkDevice>, unknown> = useQuery({
    queryKey,
    queryFn: fetchNetworkDevices,
    enabled:
      location.pathname === "/networkDevices" || location.pathname === "/admin",
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  // Query for single network device by ID
  async function fetchNetworkDevice(): Promise<NetworkDevice> {
    if (!id) throw new Error("No ID provided for network device query");
    const response = await agent.get<NetworkDevice>(
      `/api/NetworkDevices/GetNetworkDeviceById/${id}`
    );
    return response.data;
  }

  const {
    data: networkDevice,
    isLoading: isLoadingNetworkDevice,
    isError: isErrorNetworkDevice,
    error: errorNetworkDevice,
    refetch: refetchNetworkDevice,
  }: UseQueryResult<NetworkDevice, unknown> = useQuery({
    queryKey: [NETWORK_DEVICES_QUERY_KEY, id],
    queryFn: fetchNetworkDevice,
    enabled: !!id,
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  // Mutation for deleting network devices
  const deleteNetworkDevices = useMutation<void, Error, void>({
    mutationFn: async (): Promise<void> => {
      await agent.delete("/api/NetworkDevices/DeleteNetworkDevices");
    },
    onSuccess: async (): Promise<void> => {
      // Invalidate multiple related queries
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: [NETWORK_DEVICES_QUERY_KEY] }),
        queryClient.invalidateQueries({ queryKey: ["clients"] }),
        queryClient.invalidateQueries({ queryKey: ["tfPlans"] }),
        queryClient.invalidateQueries({ queryKey: ["sprVlans"] }),
      ]);
    },
  });

  return {
    networkDevices: networkDevicesResponse?.data ?? [],
    isPending,
    isError,
    error,
    pagination: networkDevicesResponse?.pagination ?? DEFAULT_PAGINATION,
    deleteNetworkDevices,
    networkDevice,
    isLoadingNetworkDevice,
    isErrorNetworkDevice,
    errorNetworkDevice,
    refetchNetworkDevices,
    refetchNetworkDevice,
  };
};

// Helper hook for using a single network device by ID
export const useNetworkDeviceById = (id?: string) => {
  const {
    networkDevice,
    isLoadingNetworkDevice,
    isErrorNetworkDevice,
    errorNetworkDevice,
    refetchNetworkDevice,
  } = useNetworkDevices(
    DEFAULT_PAGINATION_PARAMS,
    [],
    { property: "", descending: false },
    id
  );

  return {
    networkDevice,
    isLoading: isLoadingNetworkDevice,
    isError: isErrorNetworkDevice,
    error: errorNetworkDevice,
    refetch: refetchNetworkDevice,
  };
};

// Helper hook for using network devices list with default pagination and filters
export const useNetworkDevicesList = (
  pagination: PaginationParams = DEFAULT_PAGINATION_PARAMS,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams = { property: "", descending: false }
) => {
  const {
    networkDevices,
    isPending,
    isError,
    error,
    pagination: paginationData,
    refetchNetworkDevices,
  } = useNetworkDevices(pagination, filters, orderBy);

  return {
    networkDevices,
    isLoading: isPending,
    isError,
    error,
    pagination: paginationData,
    refetch: refetchNetworkDevices,
  };
};
