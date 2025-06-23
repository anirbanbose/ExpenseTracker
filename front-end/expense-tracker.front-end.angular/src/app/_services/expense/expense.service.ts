import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private httpClient = inject(HttpClient);
  constructor() { }

  addExpense(expense: any): any {
    return this.httpClient.post(`api/expense/add-new`, expense);
  }

  updateExpense(expense: any): any {
    return this.httpClient.post(`api/expense/update`, expense);
  }

  getExpenses(search: string | null = null, categoryId: string | null = null, currencyId: string | null = null, startDate: string | null = null, endDate: string | null = null, pageIndex: number = 0, pageSize: number = 10, order: number = 2, isAscending: boolean = false): any {
    return this.httpClient.get(`api/expense/search?search=${search ?? ''}&categoryId=${categoryId ?? ''}&currencyId=${currencyId ?? ''}&startDate=${startDate ?? ''}&endDate=${endDate ?? ''}&pageIndex=${pageIndex}&pageSize=${pageSize}&order=${order}&isAscending=${isAscending}`);
  }

  getExpense(id: string): any {
    return this.httpClient.get(`api/expense?id=${id}`);
  }

  deleteExpense(id: string): any {
    return this.httpClient.delete(`api/expense?id=${id}`);
  }

}
