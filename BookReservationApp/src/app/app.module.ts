import { NgModule } from "@angular/core";
import { AppComponent } from "./app.component";
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { HttpClientModule } from "@angular/common/http";
import { HomeComponent } from "./home/home.component";
import { BrowserModule } from "@angular/platform-browser";
import { NavbarComponent } from "./navbar/navbar.component";
import { AppRoutingModule } from "./app.routes";
import { RegisterComponent } from "./register/register.component";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from "./login/login.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { InputTextComponent } from "./_forms/input-text/input-text.component";
import { ToastrModule } from "ngx-toastr";
import { BooksListComponent } from "./books/books-list/books-list.component";
import { PaginationModule, PaginationConfig } from 'ngx-bootstrap/pagination';

@NgModule({
  declarations: [
    NavbarComponent,
    HomeComponent,
    AppComponent,
    LoginComponent,
    RegisterComponent,
    InputTextComponent,
    BooksListComponent,
  ],
  imports: [
    TooltipModule.forRoot(),
    HttpClientModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    ToastrModule.forRoot(),
    BrowserAnimationsModule,
    PaginationModule
  ],
  providers: [

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
