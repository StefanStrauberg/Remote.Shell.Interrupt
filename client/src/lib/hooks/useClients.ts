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
import { defaultRetry, fetchPaged, PagedResponse } from "../api/common/paged";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { DEFAULT_PAGINATION_PARAMS } from "../types/Common/DEFAULT_PAGINATION_PARAMS";

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

const CLIENTS_QUERY_KEY = "clients";
const CLIENT_BY_ID_QUERY_KEY = "clientById";
const STALE_TIME_MS = 5 * 60 * 1000; // 5 minutes

// Regex for GUID validation
const GUID_REGEX = /^[0-9a-fA-F-]{36}$/;

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
  async function fetchClients(): Promise<PagedResponse<ClientShort>> {
    const params = buildRequestParams(pagination, orderBy, filters);
    return fetchPaged<ClientShort>("/api/Clients/GetClientsByFilter", params);
  }

  const {
    data: clientsResponse,
    isLoading: isLoadingClients,
    isError: isErrorClients,
    error: errorClients,
    refetch: refetchClients,
  }: UseQueryResult<PagedResponse<ClientShort>, unknown> = useQuery({
    queryKey: clientsQueryKey,
    queryFn: fetchClients,
    enabled: !id && location.pathname === "/clients",
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  // Query for single client by ID
  async function fetchClientById(): Promise<Client> {
    if (!id) throw new Error("No ID provided for client query");

    // Numeric billing IDs resolve through the filter endpoint, GUIDs directly.
    const endpoint = isGuid
      ? `/api/Clients/GetClientById/${id}`
      : "/api/Clients/GetClientWithChildrenByFilter";

    const response = await agent.get<Client>(
      endpoint,
      isGuid
        ? undefined
        : {
            params: {
              "Filters[0].PropertyPath": "IdClient",
              "Filters[0].Operator": "Equals",
              "Filters[0].Value": String(id),
            },
          }
    );
    return response.data;
  }

  const {
    data: clientById,
    isLoading: isLoadingById,
    isError: isErrorById,
    error: errorById,
    refetch: refetchClientById,
  }: UseQueryResult<Client, unknown> = useQuery({
    queryKey: [CLIENTS_QUERY_KEY, CLIENT_BY_ID_QUERY_KEY, id],
    queryFn: fetchClientById,
    enabled: !!id,
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  // Mutation for updating clients
  const updateClients = useMutation<void, Error, void>({
    mutationFn: async (): Promise<void> => {
      await agent.put("/api/Clients/UpdateClientsLocalDb");
    },
    onSuccess: async (): Promise<void> => {
      // Invalidate all clients queries to refetch data
      await queryClient.invalidateQueries({
        queryKey: [CLIENTS_QUERY_KEY],
      });
    },
  });

  // Mutation for deleting clients
  const deleteClients = useMutation<void, Error, void>({
    mutationFn: async (): Promise<void> => {
      await agent.delete("/api/Clients/DeleteClientsLocalDb");
    },
    onSuccess: async (): Promise<void> => {
      // Invalidate multiple related queries
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: [CLIENTS_QUERY_KEY] }),
        queryClient.invalidateQueries({ queryKey: ["tfPlans"] }),
        queryClient.invalidateQueries({ queryKey: ["sprVlans"] }),
      ]);
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
