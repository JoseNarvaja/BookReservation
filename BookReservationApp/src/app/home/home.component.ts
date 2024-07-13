import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  registerActivated = false;

  registerToggle() {
    this.registerActivated = !this.registerActivated;
  }
}
