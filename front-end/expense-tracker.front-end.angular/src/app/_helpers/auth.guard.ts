import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';
import { inject } from '@angular/core';
import { of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  authService.checkAuthStatus().subscribe(isAuthenticated => {
    if (isAuthenticated) {
      console.log('User is authenticated');
    }
    else {
      console.log('User is not authenticated');
      authService.logout().subscribe({
        next: () => {
          router.navigate(['/sign-in'], { replaceUrl: true, queryParams: { returnUrl: router.url } });
        },
        error: (err: any) => {
          of(router.createUrlTree(['/sign-in']));
          console.log(err);
        }
      });
    }
  });


  return true;
};
