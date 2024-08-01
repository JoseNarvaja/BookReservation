import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
class AuthPermissionService {

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router
  ) {

  }

  canActivate(): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map(user => {
        if (user) {
          return true;
        }
        this.toastr.error("User not authenticated", "You must log in to access this resource.");
        this.router.navigate(['/']);
        return false;
      })
    );
  }
}

export const AuthGuard: CanActivateFn = (route, state) => {
  return inject(AuthPermissionService).canActivate()
};
