import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { BooksListComponent } from './books/books-list/books-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { BookDetailComponent } from './books/book-detail/book-detail.component';
import { BookDetailResolver } from './_resolvers/book-detail.resolver';
import { ReservationFormComponent } from './reservations/reservation-form/reservation-form.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '', runGuardsAndResolvers: 'always', canActivate: [AuthGuard],
    children: [
      { path: 'books', component: BooksListComponent },
      { path: 'books/:isbn', component: BookDetailComponent, resolve: { book: BookDetailResolver } },
      { path: 'books/:isbn/reserve', component: ReservationFormComponent, resolve: { book: BookDetailResolver } }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
