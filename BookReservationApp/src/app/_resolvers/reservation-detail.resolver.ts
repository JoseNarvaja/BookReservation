import { ResolveFn } from '@angular/router';
import { ReservationDto } from '../_models/reservation';
import { inject } from '@angular/core';
import { ReservationsService } from '../_services/reservations.service';
import { map } from 'rxjs';

export const reservationDetailResolver: ResolveFn<ReservationDto> = (route, state) => {
  return inject(ReservationsService).getReservation(route.paramMap.get('id')!);
};
