import { useQuery } from "@tanstack/react-query";
import { TfPlan } from "../types/TfPlan";
import agent from "../api/agent";
import { useLocation } from "react-router";

export const useTfPlans = (
  pageNumber: number = 1,
  pageSize: number = 10,
  id?: string
) => {
  const location = useLocation();

  const { data: tfPlansResponse, isPending } = useQuery({
    queryKey: ["tfPlans", pageNumber, pageSize],
    queryFn: async () => {
      const response = await agent.get<TfPlan[]>("/api/TfPlans/GetTfPlans", {
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
