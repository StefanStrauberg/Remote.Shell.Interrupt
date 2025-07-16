import { FilterDescriptor } from "../../types/Common/FilterDescriptor";
import { FilterOperator } from "../../types/Common/FilterOperator";

export const DEFAULT_FILTERS_SPRVlans: FilterDescriptor[] = [
  { PropertyPath: "UseClient", Operator: FilterOperator.Equals, Value: "true" },
];
