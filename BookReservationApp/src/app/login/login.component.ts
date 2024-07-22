import { Component, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { LoginRequest } from '../_models/loginRequest';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  @Output() cancelLogin = new EventEmitter<boolean>;
  username: string = '';
  password: string = '';

  constructor(private authService: AuthService) {

  }

  changeToRegister() {
    this.cancelLogin.emit(true);
  }

  login() {
    const loginRequest: LoginRequest = {
      userName: this.username,
      password: this.password
    };

    this.authService.login(loginRequest).subscribe(
      response => {
        console.log('Login successful', response);
      },
      error => {
        console.error('Login failed', error);
      }
    );
  }

}
