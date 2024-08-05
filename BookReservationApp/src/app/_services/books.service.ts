import { HttpClient, HttpParams, HttpResponseBase } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Book } from '../_models/book';;
import { environment } from '../environment/environment';
import { Observable, map } from 'rxjs';
import { PaginationParams } from '../_models/pagination-params';
import { ApiResponse } from '../_models/api-response';
import { PaginatedResponse } from '../_models/paginated-response';

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

  getBook(isbn: string): Observable<Book> {
    return this.http.get<ApiResponse<Book>>(this.baseUrl + '/books/' + isbn).pipe(
      map((response: ApiResponse<Book>) => {
        return response.result;
        })
    );
  }

  getPaginationParams() {
    return this.paginationParams;
  }

  setPaginationParams(paginationParams: PaginationParams) {
    this.paginationParams = paginationParams;
  }
}
