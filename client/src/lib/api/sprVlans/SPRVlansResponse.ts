import { PaginationMetadata } from "../../types/Common/PaginationMetadata";
import { SprVlan } from "../../types/SPRVlans/SprVlan";

export type SPRVlansResponse = {
  data: SprVlan[];
  pagination: PaginationMetadata;
};
