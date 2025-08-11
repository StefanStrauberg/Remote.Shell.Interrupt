import { useQuery } from "@tanstack/react-query";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { SprVlan } from "../types/SPRVlans/SprVlan";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { SPRVlansResponse } from "../api/sprVlans/SPRVlansResponse";
import { DEFAULT_PAGINATION } from "../types/Common/PaginationMetadata";
import { OrderByParams } from "../api/common/orderByParams";

export const useSPRVlans = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams
) => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;

  const queryKey = [
    "sprVlans",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
    orderBy.property,
    orderBy.descending,
  ];

  const { data: sprVlansResponse, isLoading } = useQuery<SPRVlansResponse>({
    queryKey,
    queryFn: async () => {
      const params = buildRequestParams(pagination, orderBy, filters);

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
    pagination: sprVlansResponse?.pagination ?? DEFAULT_PAGINATION,
    isLoading,
  };
};
