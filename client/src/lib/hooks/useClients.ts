import { useQuery } from "@tanstack/react-query";
import { ClientShort } from "../types/ClientShort";
import agent from "../api/agent";
import { Client } from "../types/Client";

export const useClients = (
  pageNumber: number = 1,
  pageSize: number = 10,
  id?: string
) => {
  const { data: organizationsResponse, isPending } = useQuery({
    queryKey: ["clients", pageNumber, pageSize],
    queryFn: async () => {
      const response = await agent.get<ClientShort[]>("/Clients/GetClients", {
        params: { pageNumber, pageSize },
      });
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: !id && location.pathname === "/clients",
  });

  const { data: client, isLoading: isLoadingOrganization } = useQuery({
    queryKey: ["clients", id],
    queryFn: async () => {
      const response = await agent.get<Client>(`/Clients/GetClientById/${id}`);
      return response.data;
    },
    enabled: !!id,
  });

  // Extract organizations and pagination from the gatesResponse
  const clients = organizationsResponse?.data;
  const pagination = organizationsResponse?.pagination;

  return {
    clients,
    pagination,
    isPending,
    client,
    isLoadingOrganization,
  };
};
