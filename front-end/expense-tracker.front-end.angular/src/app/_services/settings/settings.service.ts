import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private httpClient = inject(HttpClient);

  constructor() { }


  getUserPreference(): any {
    return this.httpClient.get(`api/settings/user-preference`);
  }

  saveUserPreference(userPreference: any): any {
    return this.httpClient.post(`api/settings/save-user-preference`, userPreference);
  }

}
