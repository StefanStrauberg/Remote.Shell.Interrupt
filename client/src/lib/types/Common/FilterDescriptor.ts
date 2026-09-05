import { FilterOperator } from "./FilterOperator";

export type FilterDescriptor = {
  PropertyPath: string;
  Operator: FilterOperator;
  Value: string;
};
