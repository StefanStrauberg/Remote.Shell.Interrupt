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
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { AxiosError } from "axios";

interface NetworkDevicesResponse {
  data: NetworkDevice[];
  pagination: typeof DEFAULT_PAGINATION;
}

interface UseNetworkDevicesReturn {
  networkDevices: NetworkDevice[];
  isPending: boolean;
  isError: boolean;
  error: unknown;
  pagination: typeof DEFAULT_PAGINATION;
  deleteNetworkDevices: UseMutationResult<void, Error, void, unknown>;
  networkDevice: NetworkDevice | undefined;
  isLoadingNetworkDevice: boolean;
  isErrorNetworkDevice: boolean;
  errorNetworkDevice: unknown;
  refetchNetworkDevices: () => void;
  refetchNetworkDevice: () => void;
}

// Type guard to check if error is an AxiosError
const isAxiosError = (error: unknown): error is AxiosError => {
  return error instanceof Error && "response" in error;
};

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
    "networkDevices",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  // Query for network devices list
  const {
    data: networkDevicesResponse,
    isPending,
    isError,
    error,
    refetch: refetchNetworkDevices,
  }: UseQueryResult<NetworkDevicesResponse, unknown> = useQuery({
    queryKey,
    queryFn: async (): Promise<NetworkDevicesResponse> => {
      try {
        const params = buildRequestParams(pagination, orderBy, filters);

        const response = await agent.get<NetworkDevice[]>(
          "/api/NetworkDevices/GetNetworkDevicesByFilter",
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
        console.error("Error fetching network devices:", error);
        throw error;
      }
    },
    enabled:
      location.pathname === "/networkDevices" || location.pathname === "/admin",
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      // Don't retry on 404 errors
      if (isAxiosError(error) && error.response?.status === 404) return false;
      // Retry up to 3 times for other errors
      return failureCount < 3;
    },
  });

  // Query for single network device by ID
  const {
    data: networkDevice,
    isLoading: isLoadingNetworkDevice,
    isError: isErrorNetworkDevice,
    error: errorNetworkDevice,
    refetch: refetchNetworkDevice,
  }: UseQueryResult<NetworkDevice, unknown> = useQuery({
    queryKey: ["networkDevices", id],
    queryFn: async (): Promise<NetworkDevice> => {
      try {
        if (!id) throw new Error("No ID provided for network device query");

        const response = await agent.get<NetworkDevice>(
          `/api/NetworkDevices/GetNetworkDeviceById/${id}`
        );
        return response.data;
      } catch (error) {
        console.error(`Error fetching network device with ID ${id}:`, error);
        throw error;
      }
    },
    enabled: !!id,
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      if (isAxiosError(error) && error.response?.status === 404) return false;
      return failureCount < 3;
    },
  });

  // Mutation for deleting network devices
  const deleteNetworkDevices = useMutation<void, Error, void>({
    mutationFn: async (): Promise<void> => {
      try {
        await agent.delete("/api/NetworkDevices/DeleteNetworkDevices");
      } catch (error) {
        console.error("Error deleting network devices:", error);
        throw new Error(
          error instanceof Error
            ? error.message
            : "Failed to delete network devices"
        );
      }
    },
    onSuccess: async (): Promise<void> => {
      // Invalidate multiple related queries
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ["networkDevices"] }),
        queryClient.invalidateQueries({ queryKey: ["clients"] }),
        queryClient.invalidateQueries({ queryKey: ["tfPlans"] }),
        queryClient.invalidateQueries({ queryKey: ["sprVlans"] }),
      ]);
    },
    onError: (error: Error): void => {
      console.error("Delete network devices mutation failed:", error);
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
    { pageNumber: 1, pageSize: 10 },
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
  pagination: PaginationParams = { pageNumber: 1, pageSize: 10 },
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
