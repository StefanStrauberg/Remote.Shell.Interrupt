import { useQuery } from "@tanstack/react-query";
import { Organization } from "../types/Organizations";
import agent from "../api/agent";

export const useOrganizations = (id?: string) => {
  const { data: organizations, isPending } = useQuery({
    queryKey: ["organizations"],
    queryFn: async () => {
      const response = await agent.get<Organization[]>(
        "/ClientCODs/GetClientsCOD"
      );
      return response.data;
    },
    enabled: !id && location.pathname === "/organizations",
  });

  const { data: organization, isLoading: isLoadingOrganization } = useQuery({
    queryKey: ["organizations", id],
    queryFn: async () => {
      const response = await agent.get<Organization>(
        `/ClientCODs/GetClientsCODById/${id}`
      );
      return response.data;
    },
    enabled: !!id,
  });

  return {
    organizations,
    isPending,
    organization,
    isLoadingOrganization,
  };
};
