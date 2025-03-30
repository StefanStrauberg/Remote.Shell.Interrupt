import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { Gate } from "../types/Gate";

export const useGates = (id?: string) => {
  const queryClient = useQueryClient();

  const { data: gates, isPending } = useQuery({
    queryKey: ["gates"],
    queryFn: async () => {
      const response = await agent.get<Gate[]>("/Gates/GetGates");
      return response.data;
    },
    enabled: !id && location.pathname === "/gates",
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
    gates,
    isPending,
    gate,
    isLoadingGate,
    updateGate,
    createGate,
    deleteGate,
  };
};
