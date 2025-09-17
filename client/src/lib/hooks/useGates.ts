import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { Gate } from "../types/Gates/Gate";
import { useLocation } from "react-router";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { OrderByParams } from "../api/common/orderByParams";
import { DEFAULT_PAGINATION } from "../types/Common/DEFAULT_PAGINATION";

export const useGates = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams,
  id?: string
) => {
  const queryClient = useQueryClient();
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;
  const queryKey = ["gates", pageNumber, pageSize, JSON.stringify(filters)];

  const { data: gatesResponse, isPending } = useQuery({
    queryKey,
    queryFn: async () => {
      const params = buildRequestParams(pagination, orderBy, filters);

      const response = await agent.get<Gate[]>("/api/Gates/GetGatesByFilter", {
        params,
      });
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled:
      (!id && location.pathname === "/gates") || location.pathname === "/admin",
  });

  const { data: gate, isLoading: isLoadingGate } = useQuery({
    queryKey: ["gates", id],
    queryFn: async () => {
      const response = await agent.get<Gate>(`/api/Gates/GetGateById/${id}`);
      return response.data;
    },
    enabled: !!id,
  });

  const updateGate = useMutation({
    mutationFn: async (gate: Gate) => {
      await agent.put("/api/Gates/UpdateGate", gate);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
  });

  const createGate = useMutation({
    mutationFn: async (gate: Gate) => {
      const response = await agent.post("/api/Gates/CreateGate", gate);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
  });

  const deleteGate = useMutation({
    mutationFn: async (id: string) => {
      await agent.delete(`/api/Gates/DeleteGateById/${id}`);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
  });

  return {
    gates: gatesResponse?.data ?? [],
    pagination: gatesResponse?.pagination ?? DEFAULT_PAGINATION,
    isPending,
    gate,
    isLoadingGate,
    updateGate,
    createGate,
    deleteGate,
  };
};
