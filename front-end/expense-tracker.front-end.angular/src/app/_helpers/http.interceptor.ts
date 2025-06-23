import { Injectable, inject } from '@angular/core';
import { HttpEvent, HttpRequest, HttpErrorResponse, HttpHandlerFn } from '@angular/common/http';
import { Observable, catchError, switchMap, throwError } from 'rxjs';

import { HttpInterceptorFn } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';


export const HttpRequestInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  const router = inject(Router);

  req = req.clone({
    withCredentials: true
  });


  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        const url = router.url;
        if (url.startsWith('/account')) {
          router.navigate(['/sign-in']);
        }
      }
      return throwError(() => error);
    })
  );

};

