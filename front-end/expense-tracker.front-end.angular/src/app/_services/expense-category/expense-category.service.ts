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
}
