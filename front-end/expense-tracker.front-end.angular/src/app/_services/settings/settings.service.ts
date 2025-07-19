import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }


  getUserPreference(): any {
    return this.httpClient.get(`${this.apiUrl}/settings/user-preference`);
  }

  saveUserPreference(userPreference: any): any {
    return this.httpClient.post(`${this.apiUrl}/settings/save-user-preference`, userPreference);
  }

}
