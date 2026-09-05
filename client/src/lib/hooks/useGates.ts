import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
  UseMutationResult,
} from "@tanstack/react-query";
import agent from "../api/agent";
import { Gate } from "../types/Gates/Gate";
import { useLocation } from "react-router";
import { PaginationMetadata } from "../types/Common/PaginationMetadata";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { defaultRetry, fetchPaged, PagedResponse } from "../api/common/paged";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";

interface UseGatesReturn {
  gates: Gate[];
  pagination: PaginationMetadata;
  isPending: boolean;
  isError: boolean;
  error: unknown;
  gate: Gate | undefined;
  isLoadingGate: boolean;
  isErrorGate: boolean;
  errorGate: unknown;
  updateGate: UseMutationResult<void, Error, Gate, unknown>;
  createGate: UseMutationResult<Gate, Error, Gate, unknown>;
  deleteGate: UseMutationResult<void, Error, string, unknown>;
  refetchGates: () => void;
  refetchGate: () => void;
}

const GATES_QUERY_KEY = "gates";
const STALE_TIME_MS = 5 * 60 * 1000; // 5 minutes

export const useGates = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams,
  id?: string
): UseGatesReturn => {
  const queryClient = useQueryClient();
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = [
    GATES_QUERY_KEY,
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  async function fetchGates(): Promise<PagedResponse<Gate>> {
    const params = buildRequestParams(pagination, orderBy, filters);
    return fetchPaged<Gate>("/api/Gates/GetGatesByFilter", params);
  }

  const {
    data: gatesResponse,
    isPending,
    isError,
    error,
    refetch: refetchGates,
  }: UseQueryResult<PagedResponse<Gate>, unknown> = useQuery({
    queryKey,
    queryFn: fetchGates,
    enabled:
      (!id && location.pathname === "/gates") || location.pathname === "/admin",
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  async function fetchGate(): Promise<Gate> {
    if (!id) throw new Error("No ID provided for gate query");
    const response = await agent.get<Gate>(`/api/Gates/GetGateById/${id}`);
    return response.data;
  }

  const {
    data: gate,
    isLoading: isLoadingGate,
    isError: isErrorGate,
    error: errorGate,
    refetch: refetchGate,
  }: UseQueryResult<Gate, unknown> = useQuery({
    queryKey: [GATES_QUERY_KEY, id],
    queryFn: fetchGate,
    enabled: !!id,
    staleTime: STALE_TIME_MS,
    retry: defaultRetry,
  });

  const updateGate = useMutation<void, Error, Gate>({
    mutationFn: async (gate: Gate): Promise<void> => {
      await agent.put("/api/Gates/UpdateGate", gate);
    },
    onSuccess: async (): Promise<void> => {
      await queryClient.invalidateQueries({ queryKey: [GATES_QUERY_KEY] });
    },
  });

  const createGate = useMutation<Gate, Error, Gate>({
    mutationFn: async (gate: Gate): Promise<Gate> => {
      const response = await agent.post("/api/Gates/CreateGate", gate);
      return response.data;
    },
    onSuccess: async (): Promise<void> => {
      await queryClient.invalidateQueries({ queryKey: [GATES_QUERY_KEY] });
    },
  });

  const deleteGate = useMutation<void, Error, string>({
    mutationFn: async (id: string): Promise<void> => {
      await agent.delete(`/api/Gates/DeleteGateById/${id}`);
    },
    onSuccess: async (): Promise<void> => {
      await queryClient.invalidateQueries({ queryKey: [GATES_QUERY_KEY] });
    },
  });

  return {
    gates: gatesResponse?.data ?? [],
    pagination: gatesResponse?.pagination ?? DEFAULT_PAGINATION,
    isPending,
    isError,
    error,
    gate,
    isLoadingGate,
    isErrorGate,
    errorGate,
    updateGate,
    createGate,
    deleteGate,
    refetchGates,
    refetchGate,
  };
};
