import { useQuery } from "@tanstack/react-query";
import { tfPlan } from "../types/TfPlan";
import agent from "../api/agent";

export const useTfPlans = (
  pageNumber: number = 1,
  pageSize: number = 10,
  id?: string
) => {
  const { data: tfPlansResponse, isPending } = useQuery({
    queryKey: ["tfPlans", pageNumber, pageSize],
    queryFn: async () => {
      const response = await agent.get<tfPlan[]>("/TfPlans/GetTfPlans", {
        params: { pageNumber, pageSize },
      });
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: !id && location.pathname === "/tfPlans",
  });

  // Extract organizations and pagination from the gatesResponse
  const tfPlans = tfPlansResponse?.data;
  const pagination = tfPlansResponse?.pagination;

  return {
    tfPlans,
    pagination,
    isPending,
  };
};
