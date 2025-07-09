import { PaginationMetadata } from "../../types/Common/PaginationMetadata";
import { TfPlan } from "../../types/TfPlans/TfPlan";

export type TfPlansResponse = {
  data: TfPlan[];
  pagination: PaginationMetadata;
};
