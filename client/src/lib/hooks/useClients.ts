import {
  useMutation,
  UseMutationResult,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { ClientShort } from "../types/Clients/ClientShort";
import agent from "../api/agent";
import { Client } from "../types/Clients/Client";
import { ClientFilter } from "../types/Clients/ClientFilter";
import { useLocation } from "react-router";
import { PaginationHeader } from "../types/Common/PaginationHeader";

export const useClients = (
  pageNumber: number = 1,
  pageSize: number = 10,
  filters: ClientFilter = {},
  id?: string | number
): {
  clients: ClientShort[];
  pagination: PaginationHeader;
  isLoadingClients: boolean;
  clientById: Client | undefined;
  isLoadingById: boolean;
  updateClients: UseMutationResult<void, unknown, void, unknown>;
  deleteClients: UseMutationResult<void, Error, void, unknown>;
} => {
  const queryClient = useQueryClient();
  const location = useLocation();
  const isGuid =
    typeof id === "string" && /^[0-9a-fA-F-]{36}$/.test(id.toString());

  const { data: clientsResponse, isLoading: isLoadingClients } = useQuery({
    queryKey: ["clients", pageNumber, pageSize, filters],
    queryFn: async () => {
      // Преобразуем фильтры в строку Sieve-формата: e.g. "Name@=John,Working==true"
      const filterString =
        Object.entries(filters)
          .filter(([, { op, value }]) => op && value) // Исключаем пустые значения
          .map(([key, { op, value }]) => `${key}${op}${value}`)
          .join(",") || ""; // Если фильтры пустые, возвращаем пустую строку

      console.log(filterString);

      const response = await agent.get<ClientShort[]>(
        "/api/Clients/GetShortClients",
        {
          params: { pageNumber, pageSize, Filters: filterString },
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
    pagination: clientsResponse?.pagination ?? {
      totalPages: 0,
      currentPage: 0,
    },
    isLoadingClients,
    clientById: clientById,
    isLoadingById: isLoadingById,
    updateClients,
    deleteClients,
  };
};
