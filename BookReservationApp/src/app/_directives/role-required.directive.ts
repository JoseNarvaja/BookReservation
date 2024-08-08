import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { User } from '../_models/user';
import { take } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Directive({
  selector: '[appRoleRequired]'
})
export class RoleRequiredDirective implements OnInit {
  @Input('appRoleRequired') rolesRequired: string[] = [];
  private jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService, private templateRef: TemplateRef<any>, private viewContainerRef: ViewContainerRef) {
  }

  ngOnInit(): void {
    this.authService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          const token: string | null = localStorage.getItem('token');
          if (token) {
            const tokenRole: string = this.jwtHelper.decodeToken(token)?.role;

            if (this.rolesRequired.includes(tokenRole)) {
              this.viewContainerRef.createEmbeddedView(this.templateRef);
            } else {
              this.viewContainerRef.clear();
            }
          }
        }
      },
      error: error => {
        console.log(error);
      }
    });
  }
}
