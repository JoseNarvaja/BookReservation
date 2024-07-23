import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { LoginRequest } from '../_models/loginRequest';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ApiResponse } from '../_models/apiResponse';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  @Output() cancelLogin = new EventEmitter<boolean>;
  loginForm: FormGroup = new FormGroup({});


  constructor(private authService: AuthService, private formBuilder: FormBuilder,
    private toAstrService: ToastrService, private router: Router) {

  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    })
  }

  changeToRegister() {
    this.cancelLogin.emit(true);
  }

  login() {
    const loginRequest: LoginRequest = {
      userName: this.loginForm.get('username')?.value,
      password: this.loginForm.get('password')?.value,
    };

    this.authService.login(loginRequest).subscribe(
      () => {
        this.toAstrService.success("Login successful. Welcome back!");
        this.router.navigateByUrl("/books");
      },
      (error: ApiResponse<null>) => {
        this.toAstrService.error(error.messages.join(", "), 'Login failed');
        console.error('Login failed', error);
      }
    );
  }

}