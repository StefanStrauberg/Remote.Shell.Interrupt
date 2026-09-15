import { apiPath } from "@/config/api.config";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged, PagedResponse } from "@/lib/api/common/paged";
import { OrderByParams } from "@/lib/api/common/orderByParams";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { PaginationParams } from "@/lib/types/Common/PaginationParams";
import { SprVlan } from "@/lib/types/SPRVlans/SprVlan";
import { FilterOperator } from "@/lib/types/Common/FilterOperator";

export const DEFAULT_SPR_VLAN_FILTERS: FilterDescriptor[] = [
  {
    PropertyPath: "UseClient",
    Operator: FilterOperator.Equals,
    Value: "true",
  },
];

export type SprVlansListRequest = {
  pagination: PaginationParams;
  filters?: FilterDescriptor[];
  orderBy: OrderByParams;
};

export const sprVlansApi = {
  list(request: SprVlansListRequest): Promise<PagedResponse<SprVlan>> {
    return fetchPaged(
      apiPath("SPRVlans", "GetSPRVlansByFilter"),
      buildRequestParams(
        request.pagination,
        request.orderBy,
        request.filters ?? []
      )
    );
  },
};
