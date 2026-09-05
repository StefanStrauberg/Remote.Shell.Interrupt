import { FilterDescriptor } from "../../types/Common/FilterDescriptor";
import { PaginationParams } from "../../types/Common/PaginationParams";
import { OrderByParams } from "./orderByParams";

export function buildRequestParams(
  pagination: PaginationParams,
  orderBy: OrderByParams,
  filters: FilterDescriptor[] = []
): Record<string, string | number | boolean> {
  const params: Record<string, string | number | boolean> = {
    pageNumber: pagination.pageNumber,
    pageSize: pagination.pageSize,
    orderBy: orderBy.property,
    orderByDescending: orderBy.descending,
  };

  filters.forEach((filter, index) => {
    params[`Filters[${index}].PropertyPath`] = filter.PropertyPath;
    params[`Filters[${index}].Operator`] = filter.Operator;
    params[`Filters[${index}].Value`] = filter.Value;
  });

  return params;
}
