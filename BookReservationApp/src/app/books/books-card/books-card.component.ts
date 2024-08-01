import { Component, Input } from '@angular/core';
import { Book } from '../../_models/book';

@Component({
  selector: 'app-books-card',
  templateUrl: './books-card.component.html',
  styleUrl: './books-card.component.css'
})
export class BooksCardComponent {
  @Input() book: Book | undefined;
}
