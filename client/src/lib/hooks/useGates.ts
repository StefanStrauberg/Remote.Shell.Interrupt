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
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";
import { AxiosError } from "axios";

interface GatesResponse {
  data: Gate[];
  pagination: typeof DEFAULT_PAGINATION;
}

interface UseGatesReturn {
  gates: Gate[];
  pagination: typeof DEFAULT_PAGINATION;
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

// Type guard to check if error is an AxiosError
const isAxiosError = (error: unknown): error is AxiosError => {
  return error instanceof Error && "response" in error;
};

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
    "gates",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  const {
    data: gatesResponse,
    isPending,
    isError,
    error,
    refetch: refetchGates,
  }: UseQueryResult<GatesResponse, unknown> = useQuery({
    queryKey,
    queryFn: async (): Promise<GatesResponse> => {
      try {
        const params = buildRequestParams(pagination, orderBy, filters);

        const response = await agent.get<Gate[]>(
          "/api/Gates/GetGatesByFilter",
          {
            params,
          }
        );

        // Parse pagination metadata from headers
        const paginationHeader = response.headers["x-pagination"];
        let paginationData: typeof DEFAULT_PAGINATION = DEFAULT_PAGINATION;

        if (paginationHeader && typeof paginationHeader === "string") {
          try {
            paginationData = JSON.parse(paginationHeader);
          } catch (parseError) {
            console.warn("Failed to parse pagination header:", parseError);
          }
        }

        return {
          data: response.data,
          pagination: paginationData,
        };
      } catch (error) {
        console.error("Error fetching gates:", error);
        throw error;
      }
    },
    enabled:
      (!id && location.pathname === "/gates") || location.pathname === "/admin",
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      if (isAxiosError(error) && error.response?.status === 404) return false;
      return failureCount < 3;
    },
  });

  const {
    data: gate,
    isLoading: isLoadingGate,
    isError: isErrorGate,
    error: errorGate,
    refetch: refetchGate,
  }: UseQueryResult<Gate, unknown> = useQuery({
    queryKey: ["gates", id],
    queryFn: async (): Promise<Gate> => {
      try {
        if (!id) throw new Error("No ID provided for gate query");

        const response = await agent.get<Gate>(`/api/Gates/GetGateById/${id}`);
        return response.data;
      } catch (error) {
        console.error(`Error fetching gate with ID ${id}:`, error);
        throw error;
      }
    },
    enabled: !!id,
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: (failureCount, error) => {
      if (isAxiosError(error) && error.response?.status === 404) return false;
      return failureCount < 3;
    },
  });

  const updateGate = useMutation<void, Error, Gate>({
    mutationFn: async (gate: Gate): Promise<void> => {
      try {
        await agent.put("/api/Gates/UpdateGate", gate);
      } catch (error) {
        console.error("Error updating gate:", error);
        throw new Error(
          error instanceof Error ? error.message : "Failed to update gate"
        );
      }
    },
    onSuccess: async (): Promise<void> => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
    onError: (error: Error): void => {
      console.error("Update gate mutation failed:", error);
    },
  });

  const createGate = useMutation<Gate, Error, Gate>({
    mutationFn: async (gate: Gate): Promise<Gate> => {
      try {
        const response = await agent.post("/api/Gates/CreateGate", gate);
        return response.data;
      } catch (error) {
        console.error("Error creating gate:", error);
        throw new Error(
          error instanceof Error ? error.message : "Failed to create gate"
        );
      }
    },
    onSuccess: async (): Promise<void> => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
    onError: (error: Error): void => {
      console.error("Create gate mutation failed:", error);
    },
  });

  const deleteGate = useMutation<void, Error, string>({
    mutationFn: async (id: string): Promise<void> => {
      try {
        await agent.delete(`/api/Gates/DeleteGateById/${id}`);
      } catch (error) {
        console.error("Error deleting gate:", error);
        throw new Error(
          error instanceof Error ? error.message : "Failed to delete gate"
        );
      }
    },
    onSuccess: async (): Promise<void> => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
    onError: (error: Error): void => {
      console.error("Delete gate mutation failed:", error);
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
