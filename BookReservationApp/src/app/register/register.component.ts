import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  @Output() cancelRegister = new EventEmitter<boolean>();

  changeToLogin() {
    this.cancelRegister.emit(false);
  }
}
