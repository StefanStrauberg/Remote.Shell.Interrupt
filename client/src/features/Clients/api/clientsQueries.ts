import {
  skipToken,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { clientsApi, ClientsListRequest } from "./clientsApi";
import { tfPlanKeys } from "@/features/TfPlans/api/tfPlansQueries";
import { sprVlanKeys } from "@/features/SPRVlans/api/sprVlansQueries";

export const clientKeys = {
  all: ["clients"] as const,
  lists: () => [...clientKeys.all, "list"] as const,
  list: (request: ClientsListRequest) =>
    [...clientKeys.lists(), request] as const,
  details: () => [...clientKeys.all, "detail"] as const,
  detail: (id: string | number) => [...clientKeys.details(), id] as const,
};

export function useClientsQuery(request: ClientsListRequest) {
  return useQuery({
    queryKey: clientKeys.list(request),
    queryFn: () => clientsApi.list(request),
  });
}

export function useClientQuery(id: string | number | undefined) {
  return useQuery({
    queryKey: clientKeys.detail(id ?? ""),
    queryFn:
      id !== undefined && id !== "" ? () => clientsApi.getById(id) : skipToken,
  });
}

function useInvalidateClientData() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: clientKeys.all }),
      queryClient.invalidateQueries({ queryKey: tfPlanKeys.all }),
      queryClient.invalidateQueries({ queryKey: sprVlanKeys.all }),
    ]);
}

export function useSynchronizeClientsMutation() {
  const invalidate = useInvalidateClientData();
  return useMutation<void, Error, void>({
    mutationFn: clientsApi.synchronize,
    onSuccess: invalidate,
  });
}

export function useDeleteAllClientsMutation() {
  const invalidate = useInvalidateClientData();
  return useMutation<void, Error, void>({
    mutationFn: clientsApi.removeAll,
    onSuccess: invalidate,
  });
}
