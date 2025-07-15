import {
  useMutation,
  UseMutationResult,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { ClientShort } from "../types/Clients/ClientShort";
import agent from "../api/agent";
import { Client } from "../types/Clients/Client";
import { useLocation } from "react-router";
import {
  DEFAULT_PAGINATION,
  PaginationMetadata,
} from "../types/Common/PaginationMetadata";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";

export const useClients = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  id?: string | number
): {
  clients: ClientShort[];
  pagination: PaginationMetadata;
  isLoadingClients: boolean;
  clientById: Client | undefined;
  isLoadingById: boolean;
  updateClients: UseMutationResult<void, unknown, void, unknown>;
  deleteClients: UseMutationResult<void, Error, void, unknown>;
} => {
  const location = useLocation();
  const queryClient = useQueryClient();
  const isGuid =
    typeof id === "string" && /^[0-9a-fA-F-]{36}$/.test(id.toString());
  const { pageNumber, pageSize } = pagination;
  const queryKey = ["clients", pageNumber, pageSize, JSON.stringify(filters)];

  const { data: clientsResponse, isLoading: isLoadingClients } = useQuery({
    queryKey,
    queryFn: async () => {
      const params = buildRequestParams(pagination, filters);

      const response = await agent.get<ClientShort[]>(
        "/api/Clients/GetClientsByFilter",
        {
          params,
        }
      );
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: !id && location.pathname === "/clients",
  });

  // Get client by ID
  const { data: clientById, isLoading: isLoadingById } = useQuery({
    queryKey: ["clients", id],
    queryFn: async () => {
      const endpoint = isGuid
        ? `/api/Clients/GetClientById/${id}`
        : `/api/Clients/GetClientByIdClient/${id}`;
      const response = await agent.get<Client>(endpoint);
      return response.data;
    },
    enabled: !!id,
  });

  const updateClients = useMutation({
    mutationFn: async () => {
      await agent.put("/api/Clients/UpdateClients");
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["clients"],
      });
    },
  });

  const deleteClients = useMutation({
    mutationFn: async () => {
      await agent.delete("/api/Clients/DeleteClients");
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["clients", "tfPlans", "sprVlans"],
      });
    },
  });

  return {
    clients: clientsResponse?.data ?? [],
    pagination: clientsResponse?.pagination ?? DEFAULT_PAGINATION,
    isLoadingClients,
    clientById: clientById,
    isLoadingById: isLoadingById,
    updateClients,
    deleteClients,
  };
};
