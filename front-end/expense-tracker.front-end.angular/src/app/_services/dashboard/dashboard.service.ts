import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }


  getExpenseSummary(): any {
    return this.httpClient.get(`${this.apiUrl}/dashboard/expense-summary`);
  }

  getRecentExpenses(recordCount: number = 5): any {
    return this.httpClient.get(`${this.apiUrl}/dashboard/recent-expenses?recordCount=${recordCount}`);
  }

  getSpendingChartData(): any {
    return this.httpClient.get(`${this.apiUrl}/dashboard/spending-chart`);
  }
}
