import { HttpClient, HttpParams, HttpResponseBase } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginationParams } from '../_models/paginationParams';
import { Book } from '../_models/book';;
import { environment } from '../environment/environment';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../_models/apiResponse';
import { PaginatedResponse, Pagination } from '../_models/paginatedResponse';

@Injectable({
  providedIn: 'root'
})
export class BooksService {
  baseUrl = environment.apiUrl;
  paginationParams: PaginationParams| undefined;

  constructor(private http: HttpClient) {
    this.paginationParams = new PaginationParams();
  }

  getBooks(paginationParams: PaginationParams): Observable<PaginatedResponse<Book[]>> {
    let httpParams = new HttpParams();

    httpParams = httpParams.append('pageNumber', paginationParams.pageNumber.toString());
    httpParams = httpParams.append('pageSize', paginationParams.pageSize.toString());

    return this.http.get<ApiResponse<Book[]>>(this.baseUrl + '/books', { params: httpParams, observe: 'response' }).pipe(
      map(response => {
        const paginatedBooks: PaginatedResponse<Book[]> = new PaginatedResponse<Book[]>;

        if (response.body) {
          paginatedBooks.result = response.body;
        }

        const pagination = response.headers.get('Pagination');

        if (pagination) {
          paginatedBooks.pagination = JSON.parse(pagination);
        }
        return paginatedBooks;
      })
    )
  }

  getPaginationParams() {
    return this.paginationParams;
  }

  setPaginationParams(paginationParams: PaginationParams) {
    this.paginationParams = paginationParams;
  }
}
