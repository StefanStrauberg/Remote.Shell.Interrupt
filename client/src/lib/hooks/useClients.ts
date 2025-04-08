import {
  useMutation,
  UseMutationResult,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { ClientShort } from "../types/ClientShort";
import agent from "../api/agent";
import { Client } from "../types/Client";
import { ClientFilter } from "../types/ClientFilter";
import { useLocation } from "react-router";
import { PaginationHeader } from "../types/PaginationHeader";

export const useClients = (
  pageNumber: number = 1,
  pageSize: number = 10,
  filters: ClientFilter = {},
  id?: string
): {
  clients: ClientShort[];
  pagination: PaginationHeader;
  isLoadingClients: boolean;
  clientById: Client | undefined;
  isLoadingById: boolean;
  updateClients: UseMutationResult<void, unknown, void, unknown>;
} => {
  const queryClient = useQueryClient();
  const location = useLocation();

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
        "/Clients/GetShortClients",
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
      console.log("Call clientById");
      const response = await agent.get<Client>(`/Clients/GetClientById/${id}`);
      return response.data;
    },
    enabled: !!id,
  });

  const updateClients = useMutation({
    mutationFn: async () => {
      await agent.put("/Clients/UpdateClients");
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["clients"],
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
  };
};
