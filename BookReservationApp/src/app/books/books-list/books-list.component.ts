import { Component, OnInit } from '@angular/core';
import { BooksService } from '../../_services/books.service';
import { Book } from '../../_models/book';
import { ToastrService } from 'ngx-toastr';
import { PaginatedResponse, Pagination } from '../../_models/paginated-response';
import { PaginationParams } from '../../_models/pagination-params';

@Component({
  selector: 'app-books-list',
  templateUrl: './books-list.component.html',
  styleUrl: './books-list.component.css'
})
export class BooksListComponent implements OnInit {
  books: Book[] = []
  paginationParams: PaginationParams | undefined;
  pagination: Pagination | undefined;

  constructor(private bookService: BooksService, private toAstr: ToastrService) {
    this.paginationParams = this.bookService.getPaginationParams();
  }

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks() {
    if (this.paginationParams) {
      this.bookService.setPaginationParams(this.paginationParams);
      this.bookService.getBooks(this.paginationParams).subscribe({
        next: (response: PaginatedResponse<Book[]>) => {
          if (response.result?.success) {
            this.books = response.result.result;
            this.pagination = response.pagination
          }
        }
      })
    }
  }

  pageChanged(event: any) {
    if (this.paginationParams && this.paginationParams.pageNumber !== event.page) {
      this.paginationParams.pageNumber = event.page;
      this.bookService.setPaginationParams(this.paginationParams);
      this.loadBooks();
    }
  }

}
