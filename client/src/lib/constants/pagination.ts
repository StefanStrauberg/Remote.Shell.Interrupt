import { PaginationMetadata } from "../types/Common/PaginationMetadata";

export const DEFAULT_PAGINATION: Readonly<PaginationMetadata> = Object.freeze({
  TotalCount: 0,
  PageSize: 0,
  CurrentPage: 0,
  TotalPages: 0,
  HasNext: false,
  HasPrevious: false,
});
