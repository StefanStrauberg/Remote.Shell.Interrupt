import {
  useMutation,
  UseMutationResult,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "@tanstack/react-query";
import { ClientShort } from "../types/Clients/ClientShort";
import agent from "../api/agent";
import { Client } from "../types/Clients/Client";
import { useLocation } from "react-router";
import { PaginationMetadata } from "../types/Common/PaginationMetadata";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { ClientsResponse } from "../api/Clients/ClientsResponse";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { DEFAULT_PAGINATION_PARAMS } from "../types/Common/DEFAULT_PAGINATION_PARAMS";
import { AxiosError } from "axios";

interface UseClientsReturn {
  clients: ClientShort[];
  pagination: PaginationMetadata;
  isLoadingClients: boolean;
  isErrorClients: boolean;
  errorClients: unknown;
  clientById: Client | undefined;
  isLoadingById: boolean;
  isErrorById: boolean;
  errorById: unknown;
  updateClients: UseMutationResult<void, Error, void, unknown>;
  deleteClients: UseMutationResult<void, Error, void, unknown>;
  refetchClients: () => void;
  refetchClientById: () => void;
}

// Constants for query keys to avoid typos and ensure consistency
const CLIENTS_QUERY_KEY = "clients";
const CLIENT_BY_ID_QUERY_KEY = "clientById";

// Regex for GUID validation
const GUID_REGEX = /^[0-9a-fA-F-]{36}$/;

// Type guard to check if error is an AxiosError
const isAxiosError = (error: unknown): error is AxiosError => {
  return error instanceof Error && "response" in error;
};

export const useClients = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams,
  id?: string | number
): UseClientsReturn => {
  const location = useLocation();
  const queryClient = useQueryClient();

  const isGuid = typeof id === "string" && GUID_REGEX.test(id);
  const { pageNumber, pageSize } = pagination;

  // Build query key for clients list
  const clientsQueryKey = [
    CLIENTS_QUERY_KEY,
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  // Query for clients list
  const {
    data: clientsResponse,
    isLoading: isLoadingClients,
    isError: isErrorClients,
    error: errorClients,
    refetch: refetchClients,
  }: UseQueryResult<ClientsResponse, unknown> = useQuery<ClientsResponse>({
    queryKey: clientsQueryKey,
    queryFn: async (): Promise<ClientsResponse> => {
      try {
        const params = buildRequestParams(pagination, orderBy, filters);
        const response = await agent.get<ClientShort[]>(
          "/api/Clients/GetClientsByFilter",
          { params }
        );

        // Parse pagination metadata from headers
        const paginationHeader = response.headers["x-pagination"];
        let paginationData: PaginationMetadata = DEFAULT_PAGINATION;

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
        console.error("Error fetching clients:", error);
        throw error;
      }
    },
    enabled: !id && location.pathname === "/clients",
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      // FIXED: Only don't retry on 404 errors, not all Axios errors
      if (isAxiosError(error) && error.response?.status === 404) return false;
      // Retry up to 3 times for other errors
      return failureCount < 3;
    },
  });

  // Query for single client by ID
  const {
    data: clientById,
    isLoading: isLoadingById,
    isError: isErrorById,
    error: errorById,
    refetch: refetchClientById,
  }: UseQueryResult<Client, unknown> = useQuery<Client>({
    queryKey: [CLIENTS_QUERY_KEY, CLIENT_BY_ID_QUERY_KEY, id],
    queryFn: async (): Promise<Client> => {
      try {
        if (!id) throw new Error("No ID provided for client query");

        const endpoint = isGuid
          ? `/api/Clients/GetClientById/${id}`
          : `/api/Clients/GetClientWithChildrenByFilter/?Filters[0].PropertyPath=IdClient&Filters[0].Operator=Equals&Filters[0].Value=${id}`;

        const response = await agent.get<Client>(endpoint);
        return response.data;
      } catch (error) {
        console.error(`Error fetching client with ID ${id}:`, error);
        throw error;
      }
    },
    enabled: !!id,
    staleTime: 5 * 60 * 1000, // 5 minutes
    // FIXED: Add retry logic to clientById query too
    retry: (failureCount, error) => {
      if (isAxiosError(error) && error.response?.status === 404) return false;
      return failureCount < 3;
    },
  });

  // Mutation for updating clients
  const updateClients = useMutation<void, Error, void>({
    mutationFn: async (): Promise<void> => {
      try {
        await agent.put("/api/Clients/UpdateClientsLocalDb");
      } catch (error) {
        console.error("Error updating clients:", error);
        throw new Error(
          error instanceof Error ? error.message : "Failed to update clients"
        );
      }
    },
    onSuccess: async (): Promise<void> => {
      // Invalidate all clients queries to refetch data
      await queryClient.invalidateQueries({
        queryKey: [CLIENTS_QUERY_KEY],
      });
    },
    onError: (error: Error): void => {
      console.error("Update clients mutation failed:", error);
    },
  });

  // Mutation for deleting clients
  const deleteClients = useMutation<void, Error, void>({
    mutationFn: async (): Promise<void> => {
      try {
        await agent.delete("/api/Clients/DeleteClientsLocalDb");
      } catch (error) {
        console.error("Error deleting clients:", error);
        throw new Error(
          error instanceof Error ? error.message : "Failed to delete clients"
        );
      }
    },
    onSuccess: async (): Promise<void> => {
      // Invalidate multiple related queries
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: [CLIENTS_QUERY_KEY] }),
        queryClient.invalidateQueries({ queryKey: ["tfPlans"] }),
        queryClient.invalidateQueries({ queryKey: ["sprVlans"] }),
      ]);
    },
    onError: (error: Error): void => {
      console.error("Delete clients mutation failed:", error);
    },
  });

  return {
    clients: clientsResponse?.data ?? [],
    pagination: clientsResponse?.pagination ?? DEFAULT_PAGINATION,
    isLoadingClients,
    isErrorClients,
    errorClients,
    clientById,
    isLoadingById,
    isErrorById,
    errorById,
    updateClients,
    deleteClients,
    refetchClients,
    refetchClientById,
  };
};

// Helper hook for using a single client by ID
export const useClientById = (id?: string | number) => {
  const {
    clientById,
    isLoadingById,
    isErrorById,
    errorById,
    refetchClientById,
  } = useClients(
    DEFAULT_PAGINATION_PARAMS,
    [],
    { property: "", descending: false },
    id
  );

  return {
    client: clientById,
    isLoading: isLoadingById,
    isError: isErrorById,
    error: errorById,
    refetch: refetchClientById,
  };
};

// Helper hook for using clients list with default pagination and filters
export const useClientsList = (
  pagination: PaginationParams = DEFAULT_PAGINATION_PARAMS,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams = { property: "", descending: false }
) => {
  const {
    clients,
    pagination: paginationData,
    isLoadingClients,
    isErrorClients,
    errorClients,
    refetchClients,
  } = useClients(pagination, filters, orderBy);

  return {
    clients,
    pagination: paginationData,
    isLoading: isLoadingClients,
    isError: isErrorClients,
    error: errorClients,
    refetch: refetchClients,
  };
};
