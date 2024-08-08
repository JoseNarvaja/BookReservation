import { NgModule } from "@angular/core";
import { AppComponent } from "./app.component";
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
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
import { BooksCardComponent } from "./books/books-card/books-card.component";
import { NgxSpinnerModule } from "ngx-spinner";
import { LoadingInterceptor } from "./_interceptors/loading.interceptor";
import { RequestJwtInterceptor } from "./_interceptors/request-jwt.interceptor";
import { ReservationFormComponent } from "./reservations/reservation-form/reservation-form.component";
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ReservationsListComponent } from "./reservations/reservations-list/reservations-list.component";
import { RoleRequiredDirective } from "./_directives/role-required.directive";

@NgModule({
  declarations: [
    RoleRequiredDirective,
    NavbarComponent,
    HomeComponent,
    AppComponent,
    LoginComponent,
    RegisterComponent,
    InputTextComponent,
    BooksListComponent,
    BooksCardComponent,
    ReservationFormComponent,
    ReservationsListComponent,
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
    PaginationModule,
    NgxSpinnerModule.forRoot({
      type: 'ball-spin-clockwise'
    }),
    BsDatepickerModule.forRoot()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: RequestJwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
