import { Component, OnInit } from '@angular/core';
import { ReservationsService } from '../../_services/reservations.service';
import { ReservationDto } from '../../_models/reservation';
import { ApiResponse } from '../../_models/api-response';
import { ToastrService } from 'ngx-toastr';
import { PaginatedResponse, Pagination } from '../../_models/paginated-response';
import { PaginationParams } from '../../_models/pagination-params';

@Component({
  selector: 'app-reservations-list',
  templateUrl: './reservations-list.component.html',
  styleUrl: './reservations-list.component.css'
})
export class ReservationsListComponent implements OnInit {
  reservations: ReservationDto[] = [];
  today: Date = new Date();
  paginationParams: PaginationParams = new PaginationParams();
  pagination: Pagination | undefined;

  constructor(private reservationService: ReservationsService, private toAstr: ToastrService) {
    this.paginationParams = reservationService.getPaginationParams();
  }


  ngOnInit(): void {
    this.loadReservations();
  
  }

  loadReservations(): void {
    this.reservationService.getReservations().subscribe({
      next: (response: PaginatedResponse<ReservationDto[]>) => {
        this.reservations = response.result!.result;
        this.pagination = response.pagination;
    },
    error: () => {
      this.toAstr.error("Error while loading the reservations", "Error");
    }
  });
  }

  isBeforeToday(date: Date): boolean {
    return new Date(date) > this.today;
  }

  isBetweenDates(startDate: Date, endDate: Date): boolean {
    const today = this.today;
    return new Date(startDate) <= today && today <= new Date(endDate);
  }

  pageChanged(event: any) {
    if (this.paginationParams && this.paginationParams.pageNumber !== event.page) {
      this.paginationParams.pageNumber = event.page;
      this.reservationService.setPaginationParams(this.paginationParams);
      this.loadReservations();
    }
  }
}
