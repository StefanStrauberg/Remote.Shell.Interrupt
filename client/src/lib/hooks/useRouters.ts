import {
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "@tanstack/react-query";
import { CompoundObject } from "../types/NetworkDevices/CompoundObject";
import agent from "../api/agent";
import { RouterFilter } from "../types/NetworkDevices/RouterFilter";
import { AxiosError } from "axios";

interface UseRoutersReturn {
  compoundObject: CompoundObject | undefined;
  isCompoundObject: boolean;
  isError: boolean;
  error: unknown;
  resetCache: () => void;
  refetch: () => void;
}

// Type guard to check if error is an AxiosError
const isAxiosError = (error: unknown): error is AxiosError => {
  return error instanceof Error && "response" in error;
};

export const useRouters = (
  filters: RouterFilter = {},
  isEnabled: boolean
): UseRoutersReturn => {
  const queryClient = useQueryClient();

  const fetchData = async (): Promise<CompoundObject> => {
    try {
      if (!filters.IdVlan?.value) {
        throw new Error("VLAN ID is required");
      }

      const response = await agent.get<CompoundObject>(
        `/api/NetworkDevices/GetNetworkDevicesByVlanTag/${filters.IdVlan.value}`
      );
      return response.data;
    } catch (error) {
      console.error("Error fetching routers:", error);
      throw error;
    }
  };

  const {
    data: compoundObject,
    isLoading: isCompoundObject,
    isError,
    error,
    refetch,
  }: UseQueryResult<CompoundObject, unknown> = useQuery({
    queryKey: ["mainPage", filters],
    queryFn: fetchData,
    enabled: isEnabled && !!filters.IdVlan?.value,
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      if (isAxiosError(error) && error.response?.status === 404) return false;
      return failureCount < 3;
    },
  });

  const resetCache = () => {
    queryClient.removeQueries({
      queryKey: ["mainPage"],
      exact: false,
    });
  };

  return {
    compoundObject,
    isCompoundObject,
    isError,
    error,
    resetCache,
    refetch,
  };
};
