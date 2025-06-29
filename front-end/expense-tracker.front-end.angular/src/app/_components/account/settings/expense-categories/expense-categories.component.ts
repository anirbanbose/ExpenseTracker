import { Component } from '@angular/core';
import { ExpenseCategoryListComponent } from './expense-category-list/expense-category-list.component';
import { MatCardModule } from '@angular/material/card';
import { AddEditExpenseCategoryComponent } from './add-edit-expense-category/add-edit-expense-category.component';
import { CommonModule } from '@angular/common';

enum ExpenseCategoryPageView {
  List = 1,
  Add,
  Edit,
}

@Component({
  selector: 'et-expense-categories',
  imports: [
    CommonModule,
    ExpenseCategoryListComponent,
    AddEditExpenseCategoryComponent,
    MatCardModule
  ],
  templateUrl: './expense-categories.component.html',
  styleUrl: './expense-categories.component.css'
})
export class ExpenseCategoriesComponent {
  pageTitle: string = 'Expense Categories';
  currentView: ExpenseCategoryPageView = ExpenseCategoryPageView.List;
  selectedCategoryId: string | null = null;

  onAddNew() {
    this.pageTitle = 'Add Expense Category';
    this.currentView = ExpenseCategoryPageView.Add;
    this.selectedCategoryId = null;
  }

  onEdit(categoryId: string) {
    this.pageTitle = 'Edit Expense Category';
    this.selectedCategoryId = categoryId;
    this.currentView = ExpenseCategoryPageView.Edit;
  }

  onBackToList() {
    this.currentView = ExpenseCategoryPageView.List;
    this.selectedCategoryId = null;
  }
}
