import { useQuery, useQueryClient, UseQueryResult } from "@tanstack/react-query";
import { CompoundObject } from "../types/NetworkDevices/CompoundObject";
import agent from "../api/agent";
import { RouterFilter } from "../types/NetworkDevices/RouterFilter";
import { defaultRetry } from "../api/common/paged";

interface UseRoutersReturn {
  compoundObject: CompoundObject | undefined;
  isCompoundObject: boolean;
  isError: boolean;
  error: unknown;
  resetCache: () => void;
  refetch: () => void;
}

const MAIN_PAGE_QUERY_KEY = "mainPage";
const STALE_TIME_MS = 5 * 60 * 1000; // 5 minutes

export const useRouters = (
  filters: RouterFilter = {},
  isEnabled: boolean
): UseRoutersReturn => {
  const queryClient = useQueryClient();

  async function fetchCompoundObject(): Promise<CompoundObject> {
    if (!filters.IdVlan?.value) {
      throw new Error("VLAN ID is required");
    }
    const response = await agent.get<CompoundObject>(
      `/api/NetworkDevices/GetNetworkDevicesByVlanTag/${filters.IdVlan.value}`
    );
    return response.data;
  }

  const {
    data: compoundObject,
    isLoading: isCompoundObject,
    isError,
    error,
    refetch,
  }: UseQueryResult<CompoundObject, unknown> = useQuery({
    queryKey: [MAIN_PAGE_QUERY_KEY, filters],
    queryFn: fetchCompoundObject,
    enabled: isEnabled && !!filters.IdVlan?.value,
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  const resetCache = () => {
    queryClient.removeQueries({ queryKey: [MAIN_PAGE_QUERY_KEY] });
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
