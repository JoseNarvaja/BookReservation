import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from '../../_models/book';
import { Observable, map } from 'rxjs';

@Component({
  selector: 'app-book-detail',
  templateUrl: './book-detail.component.html',
  styleUrl: './book-detail.component.css'
})
export class BookDetailComponent implements OnInit {
  book: Book = {} as Book;

  constructor(private activatedRoute: ActivatedRoute, private route: Router) { }

  ngOnInit(): void {
    this.activatedRoute.data.pipe(map(data => data['book']))
      .subscribe({
        next: book => {
          this.book = book;
        }
      });
  }

  navigateToReserve(): void {
    this.route.navigateByUrl(`/books/${this.book.isbn}/reserve`);
  }
}
