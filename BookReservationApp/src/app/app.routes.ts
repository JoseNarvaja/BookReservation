import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { BooksListComponent } from './books/books-list/books-list.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'books', component: BooksListComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
