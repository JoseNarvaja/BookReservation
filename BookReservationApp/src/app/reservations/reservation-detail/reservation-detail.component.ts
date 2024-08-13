import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReservationDto } from '../../_models/reservation';
import { ReservationsService } from '../../_services/reservations.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reservation-detail',
  templateUrl: './reservation-detail.component.html',
  styleUrl: './reservation-detail.component.css'
})
export class ReservationDetailComponent implements OnInit {
  reservation: ReservationDto = {} as ReservationDto
  today: Date = new Date();

  constructor(private activatedRoute: ActivatedRoute, private reservationService: ReservationsService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe({
      next: (data) => {
        console.log(data['reservation']);
        this.reservation = data['reservation'];
      }
    })
  }

  markAsPickedUp() {
    this.reservationService.markAsPickedUp(this.reservation.id.toString()).subscribe({
      next: () => {
        this.toastr.success("The reservation has been marked as picked up", "Reservation updated successfully");
        this.reservation.pickupDate = new Date();
      },
      error: () => {
        this.toastr.error("An unexpected error occurred while updating the reservation. Please try again later","Error");
      }
    })
  }

  markAsReturned() {
    this.reservationService.markAsReturned(this.reservation.id.toString()).subscribe({
      next: () => {
        this.toastr.success("The reservation has been marked as returned", "Reservation updated successfully");
        this.reservation.returnDate = new Date();
      },
      error: () => {
        this.toastr.error("An unexpected error occurred while updating the reservation. Please try again later", "Error");
      }
    })
  }

  isBeforeToday(date: Date | undefined): boolean {
    return date ? new Date(date) > this.today : false;
  }

  isBetweenDates(startDate: Date | undefined, endDate: Date | undefined): boolean {
    const today = this.today;
    return startDate && endDate ? new Date(startDate) <= today && today <= new Date(endDate) : false;
  }
}
