import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { Gate } from "../types/Gate";
import { GateFilter } from "../types/GateFilter";
import { useLocation } from "react-router";

export const useGates = (
  pageNumber: number = 1,
  pageSize: number = 10,
  filters: GateFilter = {},
  id?: string
) => {
  const queryClient = useQueryClient();
  const location = useLocation();

  const { data: gatesResponse, isPending } = useQuery({
    queryKey: ["gates", pageNumber, pageSize, filters],
    queryFn: async () => {
      // Преобразуем фильтры в строку Sieve-формата: e.g. "Name@=John,Working==true"
      const filterString = Object.entries(filters)
        .map(([key, { op, value }]) => `${key}${op}${value}`)
        .join(",");

      console.log(filterString);

      const response = await agent.get<Gate[]>("/Gates/GetGates", {
        params: { pageNumber, pageSize, Filters: filterString },
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
      const response = await agent.get<Gate>(`/Gates/GetGateById/${id}`);
      return response.data;
    },
    enabled: !!id,
  });

  const updateGate = useMutation({
    mutationFn: async (gate: Gate) => {
      await agent.put("/Gates/UpdateGate", gate);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
  });

  const createGate = useMutation({
    mutationFn: async (gate: Gate) => {
      const response = await agent.post("/Gates/CreateGate", gate);
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
      await agent.delete(`/Gates/DeleteGateById/${id}`);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["gates"],
      });
    },
  });

  return {
    gates: gatesResponse?.data,
    pagination: gatesResponse?.pagination,
    isPending,
    gate,
    isLoadingGate,
    updateGate,
    createGate,
    deleteGate,
  };
};
