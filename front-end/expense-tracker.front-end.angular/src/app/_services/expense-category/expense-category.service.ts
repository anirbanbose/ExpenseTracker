import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ExpenseCategoryService {
  private httpClient = inject(HttpClient);
  private apiUrl = environment.apiBaseUrl;

  constructor() { }

  getExpenseCategories(): any {
    return this.httpClient.get(`${this.apiUrl}/expense-category/all-expense-categories`);
  }

  searchExpenseCategories(search: string | null, pageIndex: number = 0, pageSize: number = 10, order: number = 1, isAscending: boolean = false): any {
    return this.httpClient.get(`${this.apiUrl}/expense-category/search?search=${search ?? ''}&pageIndex=${pageIndex}&pageSize=${pageSize}&order=${order}&isAscending=${isAscending}`);
  }

  addExpenseCategory(expenseCategory: any): any {
    return this.httpClient.post(`${this.apiUrl}/expense-category/add-new`, expenseCategory);
  }

  getExpenseCategoryById(expenseCategoryId: string): any {
    return this.httpClient.get(`${this.apiUrl}/expense-category/?id=${expenseCategoryId}`);
  }

  updateExpenseCategory(expenseCategory: any): any {
    return this.httpClient.post(`${this.apiUrl}/expense-category/update`, expenseCategory);
  }

  deleteExpenseCategory(id: string): any {
    return this.httpClient.delete(`${this.apiUrl}/expense-category?id=${id}`);
  }
}
