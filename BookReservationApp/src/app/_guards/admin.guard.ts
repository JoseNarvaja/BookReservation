import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { Observable, map, take } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})

class AdminPermissionService {
  private jwtHelper = new JwtHelperService;

  constructor(private authService: AuthService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.authService.currentUser$.pipe(map(
      user => {
        if (user) {
          const token: string | null = localStorage.getItem('token');
          if (token) {
            const tokenRole: string = this.jwtHelper.decodeToken(token)?.role;
            if (tokenRole.match('Admin')) return true;
          }
        }
        return false;
      }
    ))
  }
}


export const adminGuard: CanActivateFn = (route, state) => {
  return inject(AdminPermissionService).canActivate()
};
