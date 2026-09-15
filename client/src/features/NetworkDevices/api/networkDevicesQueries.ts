import {
  skipToken,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  networkDevicesApi,
  NetworkDevicesListRequest,
} from "./networkDevicesApi";
import { clientKeys } from "@/features/Clients/api/clientsQueries";
import { tfPlanKeys } from "@/features/TfPlans/api/tfPlansQueries";
import { sprVlanKeys } from "@/features/SPRVlans/api/sprVlansQueries";

export const networkDeviceKeys = {
  all: ["networkDevices"] as const,
  lists: () => [...networkDeviceKeys.all, "list"] as const,
  list: (request: NetworkDevicesListRequest) =>
    [...networkDeviceKeys.lists(), request] as const,
  details: () => [...networkDeviceKeys.all, "detail"] as const,
  detail: (id: string) => [...networkDeviceKeys.details(), id] as const,
};

export function useNetworkDevicesQuery(request: NetworkDevicesListRequest) {
  return useQuery({
    queryKey: networkDeviceKeys.list(request),
    queryFn: () => networkDevicesApi.list(request),
  });
}

export function useNetworkDeviceQuery(id: string | undefined) {
  return useQuery({
    queryKey: networkDeviceKeys.detail(id ?? ""),
    queryFn: id ? () => networkDevicesApi.getById(id) : skipToken,
  });
}

export function useDeleteAllNetworkDevicesMutation() {
  const queryClient = useQueryClient();
  return useMutation<void, Error, void>({
    mutationFn: networkDevicesApi.removeAll,
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: networkDeviceKeys.all }),
        queryClient.invalidateQueries({ queryKey: clientKeys.all }),
        queryClient.invalidateQueries({ queryKey: tfPlanKeys.all }),
        queryClient.invalidateQueries({ queryKey: sprVlanKeys.all }),
      ]),
  });
}
