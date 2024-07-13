import { NgModule } from "@angular/core";
import { AppComponent } from "./app.component";
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { HttpClientModule } from "@angular/common/http";
import { HomeComponent } from "./home/home.component";
import { BrowserModule } from "@angular/platform-browser";
import { NavbarComponent } from "./navbar/navbar.component";
import { AppRoutingModule } from "./app.routes";
import { RegisterComponent } from "./register/register.component";

@NgModule({
  declarations: [
    NavbarComponent,
    HomeComponent,
    AppComponent,
    RegisterComponent
  ],
  imports: [
    TooltipModule.forRoot(),
    HttpClientModule,
    BrowserModule,
    AppRoutingModule
  ],
  providers: [

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
