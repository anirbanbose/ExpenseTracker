import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {

  httpClient = inject(HttpClient);
  constructor() { }

  getCurrencies(): any {
    return this.httpClient.get(`api/currency/currencies`);
  }

  getCurrenciesForSelect(): any {
    return this.httpClient.get(`api/currency/currencies-select`);
  }

  getPreferredCurrency(): any {
    return this.httpClient.get(`api/currency/preferred-currency`);
  }
}
