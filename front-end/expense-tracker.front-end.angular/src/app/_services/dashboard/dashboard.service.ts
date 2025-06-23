import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private httpClient = inject(HttpClient);
  constructor() { }


  getExpenseSummary(yearRecordCount: number, monthRecordCount: number, categoryRecordCount: number): any {
    return this.httpClient.get(`api/dashboard/expense-summary?yearRecordCount=${yearRecordCount}&monthRecordCount=${monthRecordCount}&categoryRecordCount=${categoryRecordCount}`);
  }

  getRecentExpenses(recordCount: number = 5): any {
    return this.httpClient.get(`api/dashboard/recent-expenses?recordCount=${recordCount}`);
  }

  getSpendingChartData(): any {
    return this.httpClient.get(`api/dashboard/spending-chart`);
  }
}
