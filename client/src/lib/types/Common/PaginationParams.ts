export type PaginationParams = {
  pageNumber: number;
  pageSize: number;
};

export const DEFAULT_PAGINATION_PARAMS: PaginationParams = {
  pageNumber: 0,
  pageSize: 0,
};
