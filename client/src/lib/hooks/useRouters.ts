import { RouterFilter } from "../types/NetworkDevices/RouterFilter";
import { useQuery } from "@tanstack/react-query";
import { CompoundObject } from "../types/NetworkDevices/CompoundObject";
import agent from "../api/agent";

export const useRouters = (filters: RouterFilter = {}) => {
  const { data: compoundObject, isLoading: isCompoundObject } = useQuery({
    queryKey: ["mainPage"],
    queryFn: async () => {
      const response = await agent.get<CompoundObject>(
        `/api/NetworkDevices/GetNetworkDevicesByVlanTag/${filters.IdVlan}`
      );
      return response.data;
    },
    enabled: location.pathname === "/mainPage",
  });

  return {
    compoundObject,
    isCompoundObject,
  };
};
