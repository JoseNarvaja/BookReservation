import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private requestCount: number = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  requestSent() {
    this.requestCount++;
    this.spinnerService.show(undefined, {
      type: 'ball-spin-clockwise',
      bdColor: 'rgba(0, 0, 0, 0.7)',
      color: '#fff',
      size: 'large'
    });
  }

  responseReceived() {
    this.requestCount--;
    if (this.requestCount <= 0) {
      this.requestCount = 0;
      this.spinnerService.hide();
    }
  }
}
