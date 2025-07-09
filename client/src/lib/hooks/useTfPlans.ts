import { useQuery } from "@tanstack/react-query";
import { TfPlan } from "../types/TfPlans/TfPlan";
import agent from "../api/agent";
import { useLocation } from "react-router";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { PaginationParams } from "../types/Common/PaginationParams";
import { TfPlansResponse } from "../api/tfPlans/TfPlansResponse";
import { buildTfPlansParams } from "../api/tfPlans/buildTfPlansParams";

export const useTfPlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = []
) => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = ["tfPlans", pageNumber, pageSize, JSON.stringify(filters)];

  const { data: tfPlansResponse, isLoading } = useQuery<TfPlansResponse>({
    queryKey,
    queryFn: async () => {
      const params = buildTfPlansParams(pagination, filters);

      const response = await agent.get<TfPlan[]>(
        "/api/TfPlans/GetTfPlansByFilter",
        { params }
      );

      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: location.pathname === "/tfPlans",
  });

  return {
    tfPlans: tfPlansResponse?.data ?? [],
    pagination: tfPlansResponse?.pagination ?? {
      TotalPages: 0,
      CurrentPage: 0,
      PageSize: 0,
      TotalCount: 0,
      HasNext: false,
      HasPrevious: false,
    },
    isLoading,
  };
};
