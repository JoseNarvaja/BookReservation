import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { BooksListComponent } from './books/books-list/books-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { BookDetailComponent } from './books/book-detail/book-detail.component';
import { BookDetailResolver } from './_resolvers/book-detail.resolver';
import { ReservationFormComponent } from './reservations/reservation-form/reservation-form.component';
import { ReservationsListComponent } from './reservations/reservations-list/reservations-list.component';
import { reservationDetailResolver } from './_resolvers/reservation-detail.resolver';
import { ReservationDetailComponent } from './reservations/reservation-detail/reservation-detail.component';
import { adminGuard } from './_guards/admin.guard';
import { AdminBooksListComponent } from './admin/admin-books-list/admin-books-list.component';
import { AdminBookUpsertComponent } from './admin/admin-book-upsert/admin-book-upsert.component';
import { PhotoEditorComponent } from './admin/photo-editor/photo-editor.component';
import { AdminCopiesListComponent } from './admin/admin-copies-list/admin-copies-list.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '', runGuardsAndResolvers: 'always', canActivate: [AuthGuard],
    children: [
      { path: 'books', component: BooksListComponent },
      { path: 'reservations/:id', component: ReservationDetailComponent, resolve: { reservation: reservationDetailResolver } },
      { path: 'reservations', component: ReservationsListComponent },
      { path: 'books/:isbn', component: BookDetailComponent, resolve: { book: BookDetailResolver } },
      { path: 'books/:isbn/reserve', component: ReservationFormComponent, resolve: { book: BookDetailResolver } }
    ]
  },
  {
    path: 'admin', runGuardsAndResolvers: 'always', canActivate: [adminGuard],
    children: [
      { path: 'books', component: AdminBooksListComponent },
      { path: 'books/upsert/:isbn', component: AdminBookUpsertComponent },
      { path: 'books/upsert', component: AdminBookUpsertComponent },
      { path: 'books/upsert/:isbn/image', component: PhotoEditorComponent },
      { path: 'books/:isbn/copies', component: AdminCopiesListComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
