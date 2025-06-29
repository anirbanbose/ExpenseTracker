import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ExpenseCategoryService {

  private httpClient = inject(HttpClient);
  constructor() { }

  getExpenseCategories(): any {
    return this.httpClient.get(`api/expense-category/all-expense-categories`);
  }

  searchExpenseCategories(search: string | null, pageIndex: number = 0, pageSize: number = 10, order: number = 1, isAscending: boolean = false): any {
    return this.httpClient.get(`api/expense-category/search?search=${search ?? ''}&pageIndex=${pageIndex}&pageSize=${pageSize}&order=${order}&isAscending=${isAscending}`);
  }

  addExpenseCategory(expenseCategory: any): any {
    return this.httpClient.post(`api/expense-category/add-new`, expenseCategory);
  }

  getExpenseCategoryById(expenseCategoryId: string): any {
    return this.httpClient.get(`api/expense-category/?id=${expenseCategoryId}`);
  }

  updateExpenseCategory(expenseCategory: any): any {
    return this.httpClient.post(`api/expense-category/update`, expenseCategory);
  }

  deleteExpenseCategory(id: string): any {
    return this.httpClient.delete(`api/expense-category?id=${id}`);
  }
}
