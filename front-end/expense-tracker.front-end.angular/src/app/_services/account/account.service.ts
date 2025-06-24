import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private httpClient = inject(HttpClient);
  constructor() { }

  register(model: any): any {
    return this.httpClient.post(`api/account/register`, model);
  }

  changePassword(model: any): any {
    return this.httpClient.post(`api/account/change-password`, model);
  }

  getLoggedinUser(): any {
    return this.httpClient.get(`api/account/loggedin-user`);
  }

  emailAvailable(email: string): Observable<boolean> {
    return this.httpClient.get(`api/account/email-available?email=${email}`).pipe(
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
