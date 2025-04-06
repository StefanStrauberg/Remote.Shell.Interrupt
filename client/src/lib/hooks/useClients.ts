import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ClientShort } from "../types/ClientShort";
import agent from "../api/agent";
import { Client } from "../types/Client";

export const useClients = (
  pageNumber: number = 1,
  pageSize: number = 10,
  id?: string
) => {
  const queryClient = useQueryClient();

  const { data: clientsResponse, isPending } = useQuery({
    queryKey: ["clients", pageNumber, pageSize],
    queryFn: async () => {
      const response = await agent.get<ClientShort[]>(
        "/Clients/GetShortClients",
        {
          params: { pageNumber, pageSize },
        }
      );
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: !id && location.pathname === "/clients",
  });

  const { data: client, isLoading: isLoadingClient } = useQuery({
    queryKey: ["clients", id],
    queryFn: async () => {
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

  // Extract organizations and pagination from the gatesResponse
  const clients = clientsResponse?.data;
  const pagination = clientsResponse?.pagination;

  return {
    clients,
    pagination,
    isPending,
    client,
    isLoadingClient,
    updateClients,
  };
};
