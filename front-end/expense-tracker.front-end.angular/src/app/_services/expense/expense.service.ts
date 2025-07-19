import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }

  addExpense(expense: any): any {
    return this.httpClient.post(`${this.apiUrl}/expense/add-new`, expense);
  }

  updateExpense(expense: any): any {
    return this.httpClient.post(`${this.apiUrl}/expense/update`, expense);
  }

  getExpenses(search: string | null = null, categoryId: string | null = null, startDate: string | null = null, endDate: string | null = null, pageIndex: number = 0, pageSize: number = 10, order: number = 2, isAscending: boolean = false): any {
    return this.httpClient.get(`${this.apiUrl}/expense/search?search=${search ?? ''}&categoryId=${categoryId ?? ''}&startDate=${startDate ?? ''}&endDate=${endDate ?? ''}&pageIndex=${pageIndex}&pageSize=${pageSize}&order=${order}&isAscending=${isAscending}`);
  }

  getExpense(id: string): any {
    return this.httpClient.get(`${this.apiUrl}/expense?id=${id}`);
  }

  deleteExpense(id: string): any {
    return this.httpClient.delete(`${this.apiUrl}/expense?id=${id}`);
  }

}
