import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private httpClient = inject(HttpClient);

  constructor() { }

  getUserProfile(): any {
    return this.httpClient.get(`api/profile`);
  }

  updateProfile(profile: any): any {
    return this.httpClient.post(`api/profile/update`, profile);
  }

}
