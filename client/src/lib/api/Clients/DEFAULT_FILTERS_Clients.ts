import { FilterDescriptor } from "../../types/Common/FilterDescriptor";
import { FilterOperator } from "../../types/Common/FilterOperator";

export const DEFAULT_FILTERS_Clients: FilterDescriptor[] = [
  { PropertyPath: "Working", Operator: FilterOperator.Equals, Value: "true" },
];
