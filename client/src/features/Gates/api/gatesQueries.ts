import {
  skipToken,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { Gate, GateInput } from "@/lib/types/Gates/Gate";
import { gatesApi, GatesListRequest } from "./gatesApi";

export const gateKeys = {
  all: ["gates"] as const,
  lists: () => [...gateKeys.all, "list"] as const,
  list: (request: GatesListRequest) => [...gateKeys.lists(), request] as const,
  details: () => [...gateKeys.all, "detail"] as const,
  detail: (id: string) => [...gateKeys.details(), id] as const,
};

export function useGatesQuery(request: GatesListRequest) {
  return useQuery({
    queryKey: gateKeys.list(request),
    queryFn: () => gatesApi.list(request),
  });
}

export function useGateQuery(id: string | undefined) {
  return useQuery({
    queryKey: gateKeys.detail(id ?? ""),
    queryFn: id ? () => gatesApi.getById(id) : skipToken,
  });
}

function useInvalidateGates() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: gateKeys.all });
}

export function useCreateGateMutation() {
  const invalidate = useInvalidateGates();
  return useMutation<void, Error, GateInput>({
    mutationFn: gatesApi.create,
    onSuccess: invalidate,
  });
}

export function useUpdateGateMutation() {
  const invalidate = useInvalidateGates();
  return useMutation<void, Error, Gate>({
    mutationFn: gatesApi.update,
    onSuccess: invalidate,
  });
}

export function useDeleteGateMutation() {
  const invalidate = useInvalidateGates();
  return useMutation<void, Error, string>({
    mutationFn: gatesApi.remove,
    onSuccess: invalidate,
  });
}

export function useDeleteAllGatesMutation() {
  const invalidate = useInvalidateGates();
  return useMutation<number, Error, void>({
    mutationFn: gatesApi.removeAll,
    onSuccess: invalidate,
  });
}
