import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../_models/api-response';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CopiesService {
  private baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAvailableCopiesCount(isbn: string) {
    return this.http.get<ApiResponse<{ availableCopiesCount: number }>>(this.baseUrl + `/copies/available/${isbn}`);
  }
}
