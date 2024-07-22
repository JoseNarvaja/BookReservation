import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { LoginRequest } from '../_models/loginRequest';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { ApiResponse } from '../_models/apiResponse';
import { RegisterRequest } from '../_models/registerRequest';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(loginRequest: LoginRequest) {
    return this.http.post<ApiResponse<{ user: User; token: string }>>(this.baseUrl + '/auth/login', loginRequest).pipe(
      map((response: ApiResponse<{ user: User; token: string }>) => {
        const user = response.result.user;
        const token = response.result.token;
        if (user) {
          this.setUser(user);
          this.setToken(token);
        }
      })
    );
  }

  register(registerRequest: RegisterRequest) {
    return this.http.post<ApiResponse<null>>(this.baseUrl + '/auth/register', registerRequest);
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
  }

  setUser(user: User) {
    this.currentUserSource.next(user);
    localStorage.setItem('user', JSON.stringify(user))
  }

  setToken(token: string) {
    localStorage.setItem('token', token);
  }
}
