import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, tap } from 'rxjs/operators';
import { Constants } from '../../_helpers/constants';
import { HelperService } from '../../_helpers/helper-service/helper.service';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private helperService = inject(HelperService);
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }

  login(loginModel: { Email: string, Password: string, RememberMe: boolean }): any {
    return this.httpClient.post(`${this.apiUrl}/account/login`, loginModel)
      .pipe(
        tap((data: any) => {
          if (data && data.isLoggedIn) {
            localStorage.setItem(Constants.LOGIN_STATE_KEY, 'true');
            localStorage.setItem(Constants.LOGGED_IN_USER_KEY, JSON.stringify(data.user));
            return data.user;
          }
          else {
            return null;
          }
        }),
        catchError(this.helperService.handleError)
      );
  }

  checkAuthStatus(): Observable<boolean> {
    return this.httpClient.get(`${this.apiUrl}/account/me`).pipe(
      map(user => {
        // Authenticated
        return true;
      }),
      catchError(() => {
        // Not authenticated
        return of(false);
      })
    );
  }

  logout() {
    return this.httpClient.post(`${this.apiUrl}/account/logout`, null).pipe(
      map(message => {
        localStorage.removeItem(Constants.LOGIN_STATE_KEY);
        localStorage.removeItem(Constants.LOGGED_IN_USER_KEY);
      }),
      catchError((err: any) => {
        localStorage.removeItem(Constants.LOGIN_STATE_KEY);
        localStorage.removeItem(Constants.LOGGED_IN_USER_KEY);
        return err;
      })
    );;
  }

  getLoggedInUser(): any {
    const user = localStorage.getItem(Constants.LOGGED_IN_USER_KEY);
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }

  setLoggedinUser(user: any) {
    localStorage.setItem(Constants.LOGGED_IN_USER_KEY, JSON.stringify(user));
  }
}
