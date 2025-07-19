import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }

  getCurrencies(): any {
    return this.httpClient.get(`${this.apiUrl}/currency/currencies`);
  }

  getCurrenciesForSelect(): any {
    return this.httpClient.get(`${this.apiUrl}/currency/currencies-select`);
  }

  getPreferredCurrency(): any {
    return this.httpClient.get(`${this.apiUrl}/currency/preferred-currency`);
  }
}
