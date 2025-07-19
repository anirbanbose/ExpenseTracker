import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }

  getUserProfile(): any {
    return this.httpClient.get(`${this.apiUrl}/profile`);
  }

  updateProfile(profile: any): any {
    return this.httpClient.post(`${this.apiUrl}/profile/update`, profile);
  }

}
