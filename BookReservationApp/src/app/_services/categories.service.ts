import { HttpClient, HttpParameterCodec, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../_models/api-response';
import { PaginationParams } from '../_models/pagination-params';
import { PaginatedResponse } from '../_models/paginated-response';
import { Category } from '../_models/category';

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private baseUrl: string = environment.apiUrl;
  pagination: PaginationParams = new PaginationParams()

  constructor(private http: HttpClient) {
    this.pagination.pageSize = 50;
  }

  getCategories(): Observable<PaginatedResponse<Category[]>> {
    let headerParams: HttpParams = new HttpParams();
    headerParams = headerParams.append("pageSize", this.pagination.pageSize);
    headerParams = headerParams.append("pageNumber", this.pagination.pageNumber);

    return this.http.get<ApiResponse<Category[]>>(this.baseUrl + '/categories', { params: headerParams, observe: 'response' }).pipe(
      map(response => {
        const paginatedResponse = new PaginatedResponse<Category[]>();

        if (response.body?.success) {
          paginatedResponse.result = response.body;
        }

        const pagination = response.headers.get('Pagination');

        if (pagination) {
          paginatedResponse.pagination = JSON.parse(pagination);
        }
        return paginatedResponse;
      }));
  }

  getPagination(): PaginationParams {
    return this.pagination;
  }

  setPagination(pagination: PaginationParams) {
    this.pagination = pagination;
  }
}
