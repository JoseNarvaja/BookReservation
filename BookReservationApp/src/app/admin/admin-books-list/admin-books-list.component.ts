import { Component, OnInit } from '@angular/core';
import { Book } from '../../_models/book';
import { PaginatedResponse, Pagination } from '../../_models/paginated-response';
import { BooksParams } from '../../_models/books-params';
import { Subject, Subscription, debounceTime } from 'rxjs';
import { BooksService } from '../../_services/books.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-books-list',
  templateUrl: './admin-books-list.component.html',
  styleUrl: './admin-books-list.component.css'
})
export class AdminBooksListComponent implements OnInit {
  books: Book[] = [];
  booksParams: BooksParams = new BooksParams();
  pagination: Pagination | undefined;
  private titleSubject: Subject<string> = new Subject<string>();
  private titleSubscription: Subscription;

  constructor(private bookService: BooksService, private toAstr: ToastrService, private router: Router) {
    this.booksParams = this.bookService.getBooksParams();
    this.titleSubscription = this.titleSubject.pipe(debounceTime(600)).subscribe(title => {
      if (this.booksParams) {
        this.booksParams.title = title;
        this.loadBooks();
      }
    });
  }
  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks() {
    if (this.booksParams) {
      this.bookService.setBooksParams(this.booksParams);
      this.bookService.getBooks(this.booksParams).subscribe({
        next: (response: PaginatedResponse<Book[]>) => {
          if (response.result?.success) {
            this.books = response.result.result;
            this.pagination = response.pagination
          }
        },
        error: () => {
          this.toAstr.error("An error ocurred while loading the books. Try later", "Error");
        }
      })
    }
  }

  pageChanged(event: any) {
    if (this.booksParams && this.booksParams.paginationParams.pageNumber !== event.page) {
      this.booksParams.paginationParams.pageNumber = event.page;
      this.bookService.setBooksParams(this.booksParams);
      this.loadBooks();
    }
  }

  onTitleChange(title: string) {
    this.titleSubject.next(title);
  }

  ngOnDestroy(): void {
    this.titleSubscription.unsubscribe();
  }

  navigateToUpsert() {
    this.router.navigateByUrl("admin/books/create");
  }

}
