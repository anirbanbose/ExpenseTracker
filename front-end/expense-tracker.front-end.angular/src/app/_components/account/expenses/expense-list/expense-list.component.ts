import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { ExpenseService } from '../../../../_services/expense/expense.service';
import { PageTitleBarComponent } from '../../common/page-title-bar/page-title-bar.component';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import '../../../../_helpers/string-extensions/string-extensions';
import { ExpenseAdvancedSearchComponent } from './expense-advanced-search/expense-advanced-search.component';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog-data';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { ReportService } from '../../../../_services/report/report.service';
import { ExpenseExportDialogComponent } from './expense-export-dialog/expense-export-dialog.component';

@Component({
  selector: 'app-expense-list',
  imports: [
    CommonModule,
    PageTitleBarComponent,
    ExpenseAdvancedSearchComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.css'
})
export class ExpenseListComponent implements OnInit, AfterViewInit {
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  readonly dialog = inject(MatDialog);
  private _snackBar = inject(MatSnackBar);
  displayedColumns: string[] = ['date', 'category', 'description', 'amount', 'actions'];
  dataSource = new MatTableDataSource<any>([]);
  totalExpensesCount: number = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize = 10;
  pageIndex = 0;
  showAdvancedSearch: boolean = false;

  searchText: string | null = null;
  categoryId: string | null = null;
  startDate: string | null = null;
  endDate: string | null = null;
  sortColumn: string = 'date';
  sortDirection: "asc" | "desc" = 'desc';
  order: number = 2;
  isAscending: boolean = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  pageTitle = 'Expense List';
  breadcrumbItems = [
    { label: 'Dashboard', link: '/account/dashboard' },
    { label: 'Expenses', link: '/account/expenses/list' },
  ];
  private _helperService = inject(HelperService);
  private _expenseService = inject(ExpenseService);
  private _reportService = inject(ReportService);

  constructor() {
    this.activatedRoute.queryParamMap.subscribe(params => {
      this.showAdvancedSearch = params.get('ads') === '1';
      this.searchText = params.get('q');
      this.categoryId = params.get('cid');
      this.startDate = params.get('sd');
      this.endDate = params.get('ed');
      this.sortColumn = params.get('sa') === 'amount' ? 'amount' : params.get('sa') === 'category' ? 'category' : params.get('sa') ?? 'date';
      this.sortDirection = params.get('sdir') ? (params.get('sdir') === 'asc' ? 'asc' : 'desc') : 'desc';
      this.pageIndex = params.get('pi') ? Number(params.get('pi')) : 0;
      this.pageSize = params.get('ps') ? Number(params.get('ps')) : 10;
    });
  }

  ngOnInit(): void {
    this.getExpenses();
  }
  ngAfterViewInit() {
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
    if (this.sort) {
      this.dataSource.sort = this.sort;
    }
  }


  onEdit(expense: any) {
    this.router.navigate([`/account/expenses/edit/${expense.id}`]);
  }

  onDelete(expense: any) {
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
        this._expenseService.deleteExpense(expense.id).subscribe({
          next: () => {
            this.getExpenses();
            this._snackBar.open('Expense successfully deleted.', 'Close', {
              duration: 5000
            })
          },
          error: () => {
            this._snackBar.open('There was an issue deleting the expense. Please try again later.', 'Close', {
              duration: 5000
            })
          }
        });
      }
    });
  }

  getExpenses() {
    this.order = this.sortColumn === 'amount' ? 1 : this.sortColumn === 'date' ? 2 : this.sortColumn === 'category' ? 3 : 2;
    this.isAscending = this.sortDirection === 'asc';
    this._expenseService.getExpenses(this.searchText, this.categoryId, this.startDate, this.endDate, this.pageIndex, this.pageSize, this.order, this.isAscending).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.dataSource = response.items;
          this.totalExpensesCount = response.totalCount;
          this.paginator.pageIndex = this.pageIndex;
        }
      },
      error: (error: any) => {
        this.dataSource = new MatTableDataSource<any>([]);
        this.totalExpensesCount = 0;
        this.paginator.pageIndex = 0;

        this._snackBar.open('There was an issue fetching the expenses. Please try again later.', 'Close', {
          duration: 5000
        });
      }
    });
  }

  showDescription(description: string): string {
    return description.truncateWithEllipsis(50);
  }

  handlePageEvent(e: PageEvent) {
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this._helperService.navigateWithSearchParams({ pi: this.pageIndex, ps: this.pageSize });
    this.getExpenses();
  }

  handleSortEvent(sort: Sort) {
    this.sortColumn = sort.active;
    this.sortDirection = sort.direction ? (sort.direction === 'asc' ? 'asc' : 'desc') : 'desc';
    this._helperService.navigateWithSearchParams({ sa: sort.active, sdir: sort.direction });
    this.getExpenses();
  }

  onAddNew(): void {
    const currentUrl = this.router.url;
    this.router.navigate(['/account/expenses/add', { returnUrl: currentUrl }]);
  }

  toggleAdvancedSearch(event: Event): void {
    event.preventDefault();
    this.showAdvancedSearch = !this.showAdvancedSearch;
    this.searchText = null;
    this.categoryId = null;
    this.startDate = null;
    this.endDate = null;
    this._helperService.navigateWithSearchParams({ ads: this.showAdvancedSearch ? 1 : 0, q: this.searchText, cid: this.categoryId, sd: this.startDate, ed: this.endDate });
    this.getExpenses();
  }

  onAdvancedSearch(event: any): void {
    this.searchText = event.searchText ?? null;
    this.categoryId = event.category ?? null;
    this.startDate = event.startDate ? this._helperService.getDateOnly(event.startDate) : null;
    this.endDate = event.endDate ? this._helperService.getDateOnly(event.endDate) : null;
    this.pageIndex = 0; // Reset to first page
    this._helperService.navigateWithSearchParams({ ads: 1, q: this.searchText, cid: this.categoryId, sd: this.startDate, ed: this.endDate, pi: this.pageIndex, ps: this.pageSize });
    this.getExpenses();
  }

  onExport() {
    const dialogRef = this.dialog.open(ExpenseExportDialogComponent, {
      height: '210px',
      width: '400px',
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult !== undefined) {
        this.order = this.sortColumn === 'amount' ? 1 : this.sortColumn === 'date' ? 2 : this.sortColumn === 'category' ? 3 : 2;
        this.isAscending = this.sortDirection === 'asc';

        this._reportService.downloadExpenseExport(this.searchText, this.categoryId, this.startDate, this.endDate, this.order, this.isAscending, dialogResult).subscribe({
          next: (blob) => {
            const extension = dialogResult == 2 ? 'pdf' : 'xlsx';
            const fileName = `Expense Export.${extension}`;

            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName;
            link.click();
            window.URL.revokeObjectURL(link.href); // Clean up
          },
          error: (err) => {
            this._snackBar.open('Export failed. Please try again later.', 'Close', {
              duration: 5000
            })
          }
        });
      }
    });



  }
}
