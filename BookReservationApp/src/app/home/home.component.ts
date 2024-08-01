import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { User } from '../_models/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerActive = false;

  constructor(private authService: AuthService, private routerService: Router) {
  }

  registerToggle(active: boolean) {
    this.registerActive = active;
  }

  ngOnInit() {
    this.checkToken();
  }

  private checkToken() {
    if (this.authService.isTokenValid()) {

      const userString = localStorage.getItem('user');
      if (userString) {
        const user: User = JSON.parse(userString);
        this.authService.setUser(user);
      }

      this.routerService.navigate(['/books']);
    }
    else {
      this.authService.logout();
    }
  }
}
