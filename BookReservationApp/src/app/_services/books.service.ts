import { HttpClient, HttpParams, HttpResponseBase } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Book } from '../_models/book';;
import { environment } from '../environment/environment';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../_models/api-response';
import { PaginatedResponse } from '../_models/paginated-response';
import { BooksParams } from '../_models/books-params';

@Injectable({
  providedIn: 'root'
})
export class BooksService {
  baseUrl = environment.apiUrl;
  booksParams: BooksParams = new BooksParams;

  constructor(private http: HttpClient) {
  }

  getBooks(booksParams: BooksParams): Observable<PaginatedResponse<Book[]>> {
    console.log("REQUEST ENVIADA");
    let httpParams = new HttpParams();

    httpParams = httpParams.append('pageNumber', booksParams.paginationParams.pageNumber.toString());
    httpParams = httpParams.append('pageSize', booksParams.paginationParams.pageSize.toString());

    if (booksParams.title?.length > 1) {
      httpParams = httpParams.append('title', booksParams.title);
    }

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

  getBooksParams() {
    return this.booksParams;
  }

  setBooksParams(booksParams: BooksParams) {
    this.booksParams = booksParams;
  }
}
