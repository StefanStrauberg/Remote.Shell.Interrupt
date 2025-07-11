import { FilterDescriptor } from "../../types/Common/FilterDescriptor";
import { PaginationParams } from "../../types/Common/PaginationParams";

export function buildRequestParams(
  pagination: PaginationParams,
  filters: FilterDescriptor[] = []
): Record<string, string | number> {
  const params: Record<string, string | number> = {
    pageNumber: pagination.pageNumber,
    pageSize: pagination.pageSize,
  };

  filters.forEach((filter, index) => {
    params[`Filters[${index}].PropertyPath`] = filter.PropertyPath;
    params[`Filters[${index}].Operator`] = filter.Operator;
    params[`Filters[${index}].Value`] = filter.Value;
  });

  return params;
}
