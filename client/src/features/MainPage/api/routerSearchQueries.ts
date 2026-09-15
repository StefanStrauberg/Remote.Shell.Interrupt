import { skipToken, useQuery, useQueryClient } from "@tanstack/react-query";
import { networkDevicesApi } from "@/features/NetworkDevices/api/networkDevicesApi";

export const routerSearchKeys = {
  all: ["routerSearch"] as const,
  byVlan: (vlanId: number | undefined) =>
    [...routerSearchKeys.all, "vlan", vlanId] as const,
};

export function useRouterSearchQuery(
  vlanId: number | undefined,
  enabled: boolean
) {
  const queryClient = useQueryClient();
  const query = useQuery({
    queryKey: routerSearchKeys.byVlan(vlanId),
    queryFn:
      enabled && vlanId !== undefined
        ? () => networkDevicesApi.getByVlan(vlanId)
        : skipToken,
  });

  return {
    ...query,
    clear: () => queryClient.removeQueries({ queryKey: routerSearchKeys.all }),
  };
}
