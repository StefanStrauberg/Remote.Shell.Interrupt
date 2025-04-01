import { useQuery } from "@tanstack/react-query";
import { OrganizationShort } from "../types/OrganizationShort";
import agent from "../api/agent";

export const useOrganizations = (
  pageNumber: number = 1,
  pageSize: number = 10,
  id?: string
) => {
  const { data: organizationsResponse, isPending } = useQuery({
    queryKey: ["organizations", pageNumber, pageSize],
    queryFn: async () => {
      const response = await agent.get<OrganizationShort[]>(
        "/ClientCODs/GetClientsCOD",
        { params: { pageNumber, pageSize } }
      );
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: !id && location.pathname === "/organizations",
  });

  const { data: organization, isLoading: isLoadingOrganization } = useQuery({
    queryKey: ["organizations", id],
    queryFn: async () => {
      const response = await agent.get<OrganizationShort>(
        `/ClientCODs/GetClientsCODById/${id}`
      );
      return response.data;
    },
    enabled: !!id,
  });

  // Extract organizations and pagination from the gatesResponse
  const organizations = organizationsResponse?.data;
  const pagination = organizationsResponse?.pagination;

  return {
    organizations,
    pagination,
    isPending,
    organization,
    isLoadingOrganization,
  };
};
