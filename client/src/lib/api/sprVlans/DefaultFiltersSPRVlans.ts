import { FilterDescriptor } from "../../types/Common/FilterDescriptor";

export const DEFAULT_FILTERS_SPRVlans: FilterDescriptor[] = [
  { PropertyPath: "UseClient", Operator: "Equals", Value: "true" },
];
