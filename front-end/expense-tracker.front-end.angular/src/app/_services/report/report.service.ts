import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private httpClient = inject(HttpClient);
  constructor() { }

  downloadExpenseExport(search: string | null = null, categoryId: string | null = null, currencyId: string | null = null, startDate: string | null = null, endDate: string | null = null, order: number = 2, isAscending: boolean = false, format: 1 | 2 = 1) {
    //return this.httpClient.get(`api/report/expense-export?search=${search ?? ''}&categoryId=${categoryId ?? ''}&startDate=${startDate ?? ''}&endDate=${endDate ?? ''}&order=${order}&isAscending=${isAscending}&reportFormat=${format}`);

    const params = new HttpParams()
      .set('search', search ?? '')
      .set('categoryId', categoryId ?? '')
      .set('currencyId', currencyId ?? '')
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
}
