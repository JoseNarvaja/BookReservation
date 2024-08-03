export interface ApiResponse<T> {
  statusCode: number;
  messages: string[];
  success: boolean;
  result: T;
}
