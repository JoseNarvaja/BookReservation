import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../_models/api-response';
import { Observable } from 'rxjs';
import { ReservationDto } from '../_models/reservation';
import { ReservationCreateDto } from '../_models/reservation-create-dto';

@Injectable({
  providedIn: 'root'
})
export class ReservationsService {
  private baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  reserveBook(reservationCreateDto: ReservationCreateDto): Observable<ApiResponse<ReservationDto>> {
    console.log("DENTRO DE RESERVATION SERVICE: ");
    console.log(reservationCreateDto);
    return this.http.post<ApiResponse<ReservationDto>>(this.baseUrl + '/reservations', reservationCreateDto);
  }
}
