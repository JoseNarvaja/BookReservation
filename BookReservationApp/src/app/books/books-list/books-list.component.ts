import { Component, OnInit } from '@angular/core';
import { BooksService } from '../../_services/books.service';
import { Book } from '../../_models/book';
import { PaginationParams } from '../../_models/paginationParams';
import { PaginatedResponse, Pagination } from '../../_models/paginatedResponse';
import { ToastrService } from 'ngx-toastr';

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
    console.log("PAGINATION PARAMS:\nPNumber" + this.paginationParams?.pageNumber + "\nPNumber" + this.paginationParams?.pageSize);
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
