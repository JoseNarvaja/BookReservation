import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse } from '../_models/api-response';
import { Observable, map } from 'rxjs';
import { ReservationDto } from '../_models/reservation';
import { ReservationCreateDto } from '../_models/reservation-create-dto';
import { PaginatedResponse, Pagination } from '../_models/paginated-response';
import { PaginationParams } from '../_models/pagination-params';

@Injectable({
  providedIn: 'root'
})
export class ReservationsService {
  private baseUrl: string = environment.apiUrl;
  private paginationParams: PaginationParams = new PaginationParams();
  private pagination: Pagination | undefined;

  constructor(private http: HttpClient) {
  }

  reserveBook(reservationCreateDto: ReservationCreateDto): Observable<ApiResponse<ReservationDto>> {
    return this.http.post<ApiResponse<ReservationDto>>(this.baseUrl + '/reservations', reservationCreateDto);
  }

  getReservations(): Observable<PaginatedResponse<ReservationDto[]>> {
    let httpParams = new HttpParams();

    httpParams = httpParams.append('pageNumber', this.paginationParams.pageNumber.toString());
    httpParams = httpParams.append('pageSize', this.paginationParams.pageSize.toString());

    return this.http.get<ApiResponse<ReservationDto[]>>(this.baseUrl + '/reservations', { params: httpParams, observe: 'response' }).pipe(
      map(response => {
        const paginatedReservations: PaginatedResponse<ReservationDto[]> = new PaginatedResponse<ReservationDto[]>;

        if (response.body) {
          paginatedReservations.result = response.body;
        }

        const pagination = response.headers.get('Pagination');

        if (pagination) {
          paginatedReservations.pagination = JSON.parse(pagination);
        }
        return paginatedReservations;
      }));
  }

  getPaginationParams() {
    return this.paginationParams;
  }

  setPaginationParams(pagination: PaginationParams) {
    this.paginationParams = pagination;
  }
}
