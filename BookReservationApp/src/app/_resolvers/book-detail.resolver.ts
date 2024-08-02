import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { Book } from "../_models/book";
import { BooksService } from "../_services/books.service";

export const BookDetailResolver: ResolveFn<Book> = (route, state) => {
  const isbn = route.paramMap.get('isbn')!;

  return inject(BooksService).getBook(isbn);
}
