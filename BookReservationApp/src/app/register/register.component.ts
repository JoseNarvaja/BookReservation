import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { ApiResponse } from '../_models/apiResponse';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter<boolean>();
  registerForm: FormGroup = new FormGroup({});
  validationErrors: string[] | undefined;

  constructor(private formBuilder: FormBuilder, private authService: AuthService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeRegisterForm();
  }

  changeToLogin() {
    this.cancelRegister.emit(false);
  }

  initializeRegisterForm() {
    this.registerForm = this.formBuilder.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      username: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8),
        Validators.maxLength(32)]],
      confirmPassword: ['', [Validators.required, this.samePassword('password')]]
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    });
  }

  samePassword(match: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(match)?.value ? null : { notMatching: true }
    }
  }

  register() {
    const values = { ...this.registerForm.value };
    console.log(values);
    this.authService.register(values).subscribe({
      next: response => {
        console.log(response);
        this.toastr.success("Log in to continue", "Registration successful");
        this.changeToLogin();
      },
      error: (error: ApiResponse<null>) => {
        this.toastr.error("An error ocurred while registering", "Error");
        this.validationErrors = error.messages;
      }
    });
  }
}
