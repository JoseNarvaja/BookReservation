import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse } from '../_models/api-response';
import { Observable, map } from 'rxjs';
import { PaginatedResponse, Pagination } from '../_models/paginated-response';
import { Copy } from '../_models/copy';
import { PaginationParams } from '../_models/pagination-params';
import { CopyUpsertDto} from '../_models/copy-upsert-dto';

@Injectable({
  providedIn: 'root'
})
export class CopiesService {
  private baseUrl: string = environment.apiUrl;
  paginationParams: PaginationParams = new PaginationParams();
  constructor(private http: HttpClient) { }

  getPagination(): PaginationParams {
    return this.paginationParams
  }

  setPagination(pagination: PaginationParams): void {
    this.paginationParams = pagination;
  }

  getCopies(isbn: string, paginationParams: PaginationParams) {
    let httpParams = new HttpParams();

    httpParams = httpParams.append('pageNumber', paginationParams.pageNumber.toString());
    httpParams = httpParams.append('pageSize', paginationParams.pageSize.toString());

    return this.http.get<ApiResponse<Copy[]>>(this.baseUrl + "/copies/", { params: httpParams, observe: 'response' }).pipe(
      map(response => {
        const paginatedCopies: PaginatedResponse<Copy[]> = new PaginatedResponse<Copy[]>;

        if (response.body) {
          paginatedCopies.result = response.body;
        }

        const pagination = response.headers.get('Pagination');

        if (pagination) {
          paginatedCopies.pagination = JSON.parse(pagination);
        }
        return paginatedCopies;
      }));
  }

  getAvailableCopiesCount(isbn: string) {
    return this.http.get<ApiResponse<{ availableCopiesCount: number }>>(this.baseUrl + `/copies/available/${isbn}`);
  }

  createCopy(copy: CopyUpsertDto) {
    return this.http.post<ApiResponse<Copy>>(this.baseUrl + '/copies', copy);
  }

  deleteCopy(barcode: string) {
    return this.http.delete(this.baseUrl + '/copies/' + barcode);
  }

  updateCopy(barcode: string, copy: CopyUpsertDto) {
    return this.http.put(this.baseUrl + '/copies/' + barcode, copy);
  }

}
