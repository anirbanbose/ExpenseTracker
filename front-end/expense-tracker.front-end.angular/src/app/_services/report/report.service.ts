import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private httpClient = inject(HttpClient);

  constructor() { }

  downloadExpenseExport(search: string | null = null, categoryId: string | null = null, startDate: string | null = null, endDate: string | null = null, order: number = 2, isAscending: boolean = false, format: 1 | 2 = 1) {
    const params = new HttpParams()
      .set('search', search ?? '')
      .set('categoryId', categoryId ?? '')
      .set('startDate', startDate ?? '')
      .set('endDate', endDate ?? '')
      .set('order', order)
      .set('isAscending', isAscending)
      .set('reportFormat', format);

    return this.httpClient.get(`api/report/expense-export`, {
      params,
      responseType: 'blob' // Important: this tells Angular to treat response as binary
    });
  }

  getMinMaxDates() {
    return this.httpClient.get<{ minDate: any, maxDate: any }>(`api/report/min-max-dates`);
  }

  downloadExpenseReport(reportType: 1 | 2, year: number, month: number | null, format: 1 | 2 = 1) {
    const params = new HttpParams()
      .set('reportType', reportType)
      .set('year', year)
      .set('month', month ?? '')
      .set('reportFormat', format);

    return this.httpClient.get(`api/report/expense-report`, {
      params,
      responseType: 'blob' // Important: this tells Angular to treat response as binary
    });
  }
}
