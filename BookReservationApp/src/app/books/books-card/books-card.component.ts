import { Component, Input } from '@angular/core';
import { Book } from '../../_models/book';
import { Router } from '@angular/router';

@Component({
  selector: 'app-books-card',
  templateUrl: './books-card.component.html',
  styleUrl: './books-card.component.css'
})
export class BooksCardComponent {
  @Input() book: Book | undefined;

  constructor(private router: Router) {

  }

  navigateToBookDetails() {
    this.router.navigateByUrl('/books/' + this.book?.isbn);
  }
}
