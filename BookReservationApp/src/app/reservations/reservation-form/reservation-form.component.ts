import { Component, OnInit } from '@angular/core';
import { Book } from '../../_models/book';
import { ActivatedRoute, Router } from '@angular/router';
import { map } from 'rxjs';
import { CopiesService } from '../../_services/copies.service';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ReservationsService } from '../../_services/reservations.service';
import { ReservationCreateDto } from '../../_models/reservation-create-dto';
import { ToastrService } from 'ngx-toastr';
import { ApiResponse } from '../../_models/api-response';

@Component({
  selector: 'app-reservation-form',
  templateUrl: './reservation-form.component.html',
  styleUrl: './reservation-form.component.css'
})
export class ReservationFormComponent implements OnInit {
  minDate: Date;
  maxDate: Date;
  public isAnyAvailable = false;
  book: Book = {} as Book;
  reservationForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];

  constructor(private activatedRoute: ActivatedRoute, private copiesService: CopiesService,
    private reservationService: ReservationsService, private toastr: ToastrService,
    private router: Router) {
    this.minDate = new Date();
    this.maxDate = new Date();
    this.maxDate.setDate(this.maxDate.getDate() + 150);
  }

  ngOnInit(): void {

    this.reservationForm = new FormGroup({
      reservationRange: new FormControl('', [Validators.required, this.validateDateRange()])
    })

    this.activatedRoute.data.pipe(map(data => data['book']))
      .subscribe({
        next: book => {
          this.book = book;
        }
      });

    if (this.book) {
      this.copiesService.getAvailableCopiesCount(this.book.isbn).subscribe({
        next: response => {
          this.isAnyAvailable = (response.result.availableCopiesCount > 0) ? true : false;
        }
      })
    }
    else {
      this.toastr.error("Error", "The book wasn't found");
      this.router.navigateByUrl("/books");
    }
  }

  validateDateRange(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const dates = control.value;

      if (dates && dates.length === 2) {
        const dateStart = new Date(dates[0])
        const dateEnd = new Date(dates[1])
        const diffDays = (dateEnd.getTime() - dateStart.getTime()) / (1000 * 3600 * 24);

        if (diffDays > 90) {
          return { dateRangeInvalid: true }
        }
      }
      return null;
    }
  }

  reserveBook() {
    this.validationErrors = [];

    const dates = this.reservationForm.get('reservationRange')?.value;
    if (dates && dates.length === 2) {
      const reservationCreateDto: ReservationCreateDto = {
        ISBN: this.book.isbn,
        reservationDate: new Date(dates[0]).toISOString(),
        reservationEnd: new Date(dates[0]).toISOString()
      }

      this.reservationService.reserveBook(reservationCreateDto).subscribe({
        next: response => {
          this.toastr.success("Your reservation was placed", "Book reserved successfully");
          this.router.navigateByUrl("/books");
        },
        error: (response: ApiResponse<null>) => {
          this.toastr.error("An error ocurred while placing your reservation", "Error");
          this.validationErrors = response.messages;
        }
      });
    }
  }
}
