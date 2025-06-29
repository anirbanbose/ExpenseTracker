import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, EventEmitter, inject, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { HelperService } from '../../../../../_helpers/helper-service/helper.service';
import { ExpenseCategoryService } from '../../../../../_services/expense-category/expense-category.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmDialogData } from '../../../../shared/confirm-dialog/confirm-dialog-data';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../../shared/confirm-dialog/confirm-dialog.component';
import { AlertDialogData } from '../../../../shared/alert-dialog/alert-dialog-data';
import { AlertDialogComponent } from '../../../../shared/alert-dialog/alert-dialog.component';

@Component({
  selector: 'et-settings-expense-category-list',
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
  ],
  templateUrl: './expense-category-list.component.html',
  styleUrl: './expense-category-list.component.css'
})
export class ExpenseCategoryListComponent implements OnInit, AfterViewInit {
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  searchText: string | null = '';
  totalCategoryCount: number = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize = 5;
  pageIndex = 0;
  displayedColumns: string[] = ['name', 'expenseCount', 'actions'];
  dataSource = new MatTableDataSource<any>([]);
  sortColumn: string = 'name';
  sortDirection: "asc" | "desc" = 'asc';
  order: number = 1;
  isAscending: boolean = true;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  @Output() addNew: EventEmitter<void> = new EventEmitter();
  @Output() editCategory: EventEmitter<any> = new EventEmitter();

  private _snackBar = inject(MatSnackBar);
  readonly dialog = inject(MatDialog);
  private _helperService = inject(HelperService);
  private _expenseCategoryService = inject(ExpenseCategoryService);

  constructor() {
    this.activatedRoute.queryParamMap.subscribe(params => {
      this.searchText = params.get('q');
      this.sortColumn = params.get('sa') === 'amount' ? 'amount' : params.get('sa') === 'category' ? 'category' : params.get('sa') ?? 'date';
      this.sortDirection = params.get('sdir') ? (params.get('sdir') === 'asc' ? 'asc' : 'desc') : 'asc';
      this.pageIndex = params.get('pi') ? Number(params.get('pi')) : 0;
      this.pageSize = params.get('ps') ? Number(params.get('ps')) : 5;
    });
  }


  ngOnInit(): void {
    this.getExpenseCategories();
  }

  ngAfterViewInit() {
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
    if (this.sort) {
      this.dataSource.sort = this.sort;
    }
  }

  getExpenseCategories() {
    this.order = this.sortColumn === 'expenseCount' ? 2 : 1;
    this.isAscending = this.sortDirection === 'asc';
    this._expenseCategoryService.searchExpenseCategories(this.searchText, this.pageIndex, this.pageSize, this.order, this.isAscending).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.dataSource = response.items;
          this.totalCategoryCount = response.totalCount;
          if (this.paginator) {
            this.paginator.pageIndex = this.pageIndex;
          }
        }
      },
      error: (error: any) => {
        this._snackBar.open('There was an issue fetching the expenses. Please try again later.', 'Close', {
          duration: 5000
        });
      }
    });
  }



  onSearch() {
    this.pageIndex = 0; // Reset to first page
    this._helperService.navigateWithSearchParams({ q: this.searchText, pi: this.pageIndex, ps: this.pageSize });
    this.getExpenseCategories();
  }

  onAddNew() {
    this.addNew.emit();
  }

  onEdit($event: any) {
    this.editCategory.emit($event.id);
  }

  onDelete($event: any) {
    if (!this.checkIfExpenseCategoryHasExpenses($event)) {
      return; // Cannot delete category with expenses
    }
    let data: ConfirmDialogData = {
      title: 'Delete Expense?',
      message: 'Are you sure you want to delete this expense?',
      OkButtonText: "Yes",
      cancelButtonText: "No"
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      height: '210px',
      width: '400px',
      data: data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this._expenseCategoryService.deleteExpenseCategory($event.id).subscribe({
          next: () => {
            this.getExpenseCategories();
            this._snackBar.open('Expense category successfully deleted.', 'Close', {
              duration: 5000
            })
          },
          error: () => {
            this._snackBar.open('There was an issue deleting the expense category. Please try again later.', 'Close', {
              duration: 5000
            })
          }
        });
      }
    });
  }

  checkIfExpenseCategoryHasExpenses(expenseCategory: any): boolean {
    if (expenseCategory && expenseCategory.expenseCount > 0) {
      let data: AlertDialogData = {
        title: 'Expenses Found',
        message: 'This category has expenses associated with it. Please delete the expenses before deleting the category.',
        OkButtonText: "Ok"
      }
      const dialogRef = this.dialog.open(AlertDialogComponent, {
        height: '250px',
        width: '400px',
        data: data
      });
      return false;  // Cannot delete category with expenses
    }
    return true;
  }


  onClearSearch() {
    this.searchText = '';
    this.onSearch();
  }

  handlePageEvent(e: PageEvent) {
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this._helperService.navigateWithSearchParams({ pi: this.pageIndex, ps: this.pageSize });
    this.getExpenseCategories();
  }

  handleSortEvent(sort: Sort) {
    this.sortColumn = sort.active;
    this.sortDirection = sort.direction ? (sort.direction === 'asc' ? 'asc' : 'desc') : 'desc';
    this._helperService.navigateWithSearchParams({ sa: sort.active, sdir: sort.direction });
    this.getExpenseCategories();
  }

  goToExpenseList($event: any) {
    this.router.navigate(['/account/expenses/list'], { queryParams: { ads: 1, cid: $event.id } });
  }
}
