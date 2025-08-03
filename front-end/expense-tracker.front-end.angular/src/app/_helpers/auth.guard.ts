import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';
import { inject } from '@angular/core';
import { of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  authService.checkAuthStatus().subscribe(isAuthenticated => {
    if (isAuthenticated) {

    }
    else {
      authService.logout().subscribe({
        next: () => {
          router.navigate(['/sign-in'], { replaceUrl: true, queryParams: { returnUrl: router.url } });
        },
        error: (err: any) => {
          of(router.createUrlTree(['/sign-in']));
        }
      });
    }
  });


  return true;
};
