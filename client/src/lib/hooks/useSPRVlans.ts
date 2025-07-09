import { useQuery } from "@tanstack/react-query";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { SprVlan } from "../types/SPRVlans/SprVlan";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildTfPlansParams } from "../api/tfPlans/buildTfPlansParams";

export const useSPRVlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = []
) => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = ["sprVlans", pageNumber, pageSize, JSON.stringify(filters)];

  const { data: sprVlansResponse, isLoading } = useQuery({
    queryKey,
    queryFn: async () => {
      const params = buildTfPlansParams(pagination, filters);

      const response = await agent.get<SprVlan[]>(
        "/api/SPRVlans/GetSPRVlansByFilter",
        {
          params,
        }
      );
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: location.pathname === "/sprVlans",
  });

  return {
    sprVlans: sprVlansResponse?.data ?? [],
    pagination: sprVlansResponse?.pagination ?? {
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
