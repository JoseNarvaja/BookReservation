import { ApiResponse } from "./apiResponse";

export interface Pagination {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResponse<T> {
  result?: ApiResponse<T>
  pagination?: Pagination;
}
