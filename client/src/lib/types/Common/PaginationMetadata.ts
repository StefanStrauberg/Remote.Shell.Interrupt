export type PaginationMetadata = {
  TotalCount: number;
  PageSize: number;
  CurrentPage: number;
  TotalPages: number;
  HasNext: boolean;
  HasPrevious: boolean;
};

export const DEFAULT_PAGINATION: PaginationMetadata = {
  TotalCount: 0,
  PageSize: 0,
  CurrentPage: 0,
  TotalPages: 0,
  HasNext: false,
  HasPrevious: false,
};
