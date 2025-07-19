import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;
  constructor() { }

  register(model: any): any {
    return this.httpClient.post(`${this.apiUrl}/account/register`, model);
  }

  changePassword(model: any): any {
    return this.httpClient.post(`${this.apiUrl}/account/change-password`, model);
  }

  getLoggedinUser(): any {
    return this.httpClient.get(`${this.apiUrl}/account/loggedin-user`);
  }

  emailAvailable(email: string): Observable<boolean> {
    return this.httpClient.get(`${this.apiUrl}/account/email-available?email=${email}`).pipe(
      map(result => {
        if (result) {
          return true;
        }
        return false;
      }),
      catchError(() => {
        return of(false);
      })
    );
  }
}
