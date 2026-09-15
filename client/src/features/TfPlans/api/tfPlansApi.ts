import { apiPath } from "@/config/api.config";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged, PagedResponse } from "@/lib/api/common/paged";
import { OrderByParams } from "@/lib/api/common/orderByParams";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { PaginationParams } from "@/lib/types/Common/PaginationParams";
import { TfPlan } from "@/lib/types/TfPlans/TfPlan";

export type TfPlansListRequest = {
  pagination: PaginationParams;
  filters?: FilterDescriptor[];
  orderBy: OrderByParams;
};

export const tfPlansApi = {
  list(request: TfPlansListRequest): Promise<PagedResponse<TfPlan>> {
    return fetchPaged(
      apiPath("TfPlans", "GetTfPlansByFilter"),
      buildRequestParams(
        request.pagination,
        request.orderBy,
        request.filters ?? []
      )
    );
  },
};
