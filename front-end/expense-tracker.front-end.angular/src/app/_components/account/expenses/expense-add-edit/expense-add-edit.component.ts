import { CommonModule, AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { ExpenseCategoryService } from '../../../../_services/expense-category/expense-category.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { PageTitleBarComponent } from '../../common/page-title-bar/page-title-bar.component';
import { CurrencyService } from '../../../../_services/currency/currency.service';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatIconModule } from '@angular/material/icon';
import { provideNativeDateAdapter } from '@angular/material/core';
import { formatISO } from 'date-fns';
import { ExpenseService } from '../../../../_services/expense/expense.service';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  standalone: true,
  selector: 'app-expense-add-edit',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PageTitleBarComponent,
    MatCardModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatAutocompleteModule,
    MatIconModule,
    MatDatepickerModule,
    AsyncPipe
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './expense-add-edit.component.html',
  styleUrl: './expense-add-edit.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseAddEditComponent implements OnInit {
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private _snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  private _helperService = inject(HelperService);
  private _expenseCategoryService = inject(ExpenseCategoryService);
  private _expenseService = inject(ExpenseService);
  private _currencyService = inject(CurrencyService);
  filteredCategoryOptions!: Observable<any[]>;
  pageTitle = 'Add Expense';
  breadcrumbItems = [
    { label: 'Dashboard', link: '/account/dashboard' },
    { label: 'Expenses', link: '/account/expenses/list' },
    { label: 'Add Expense', link: null },
  ];

  expenseCategories: any[] = [];
  currencies: any[] = [];
  expenseForm: FormGroup = this.fb.group({
    id: [null],
    amount: [null, [Validators.required]],
    description: [null],
    expenseDate: [new Date(), [Validators.required]],
    category: [null, [Validators.required]],
    currency: [null, [Validators.required]],
  });
  isFormSubmitted = false;

  id: string | null = null;


  ngOnInit(): void {
    this.getExpenseCategories();
    this.getCurrencies();
    this.getPreferredCurrency();

    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      if (this.id) {
        this.pageTitle = 'Edit Expense';
        this.breadcrumbItems[2].label = 'Edit Expense';
        this.getExpense();
      }
    });
  }

  private _filterExpenseCategories(value: any): any[] {
    const filterValue = value ? value?.toLowerCase() : '';
    let retVal = this.expenseCategories.filter(option => option?.name.toLowerCase().includes(filterValue));
    return retVal;
  }

  get expenseCategoryControl(): any {
    return this.expenseForm.get('category');
  }

  onSubmit(): void {
    this.isFormSubmitted = true;

    if (this.expenseForm.valid) {
      const formValue = this.expenseForm.value;

      let modelData: any = {
        amount: formValue.amount,
        description: formValue.description,
        categoryId: formValue.category?.id,
        currencyId: formValue.currency,
        expenseDate: this._helperService.getDateOnly(formValue.expenseDate),
      };
      if (this.id && formValue.id) {
        modelData.id = formValue.id;
        this._expenseService.updateExpense(modelData).subscribe({
          next: (data: any) => {
            if (data) {
              console.log(data);
              this._snackBar.open(data.message, 'Close', {
                duration: 5000
              });
              this.router.navigate(['/account/expenses/list']);
            }
          },
          error: (err: HttpErrorResponse) => {
            if (err.status == 401) {
              //this._helperService.handle401Error(this._snackBar, this._authService);
            }
            else if (err.error?.errorMessage) {
              this._snackBar.open(err.error?.errorMessage, 'Close', {
                duration: 5000
              });
            }
            else {
              this._snackBar.open('There was an issue in updating the expense. Please try again later.', 'Close', {
                duration: 5000
              });
            }
          }
        });
      }
      else {
        this._expenseService.addExpense(modelData).subscribe({
          next: (data: any) => {
            if (data) {
              this._snackBar.open(data.message, 'Close', {
                duration: 5000
              });
              this.router.navigate(['/account/expenses/list']);
            }
          },
          error: (err: HttpErrorResponse) => {
            if (err.status == 401) {
              //this._helperService.handle401Error(this._snackBar, this._authService);
            }
            else if (err.error?.errorMessage) {
              this._snackBar.open(err.error?.errorMessage, 'Close', {
                duration: 5000
              });
            }
            else {
              this._snackBar.open('There was an issue in updating the expense. Please try again later.', 'Close', {
                duration: 5000
              });
            }
          }
        });
      }

    }
  }

  onCancel(): void {
    this.router.navigate(['/account/expenses/list']);
  }

  getExpenseCategories(): void {
    this._expenseCategoryService.getExpenseCategories().subscribe({
      next: (data: any) => {
        if (data) {
          this.expenseCategories = data;
          this.filteredCategoryOptions = this.expenseCategoryControl.valueChanges.pipe(
            startWith(null),
            map((value: any) => {
              return this._filterExpenseCategories(value || null);
            }),
          );
        }
        else {
        }
      },
      error: (err: HttpErrorResponse) => {
        if (err.status == 401) {
          //this._helperService.handle401Error(this._snackBar, this._authService);
        }
        else if (err.error?.errorMessage) {

        }
        else {

        }
      }
    });
  }

  getCurrencies(): void {
    this._currencyService.getCurrenciesForSelect().subscribe({
      next: (data: any) => {
        if (data && data.length > 0) {
          this.currencies = data;
        }
      },
      error: (err: HttpErrorResponse) => {
        if (err.status == 401) {
          //this._helperService.handle401Error(this._snackBar, this._authService);
        }
        else if (err.error?.errorMessage) {

        }
        else {

        }
      }
    });
  }

  displayFn(obj: any): string {
    return obj ? obj.name : '';
  }


  getPreferredCurrency(): void {
    this._currencyService.getPreferredCurrency().subscribe({
      next: (data: any) => {
        if (data) {
          this.expenseForm.patchValue({
            currency: data.id
          });
        }
        else {
        }
      },
      error: (err: HttpErrorResponse) => {
        if (err.status == 401) {
          //this._helperService.handle401Error(this._snackBar, this._authService);
        }
        else if (err.error?.errorMessage) {

        }
        else {

        }
      }
    });
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.expenseForm, controlName, this.isFormSubmitted);
  }

  getExpense() {
    if (!this.id) return;
    this._expenseService.getExpense(this.id).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.expenseForm.patchValue({
            id: response.value.id,
            amount: response.value.expenseAmount,
            description: response.value.description,
            category: response.value.category,
            currency: response.value.currencyId,
            expenseDate: response.value.expenseDate
          });
        }
      },
      error: (error: any) => {
        console.log(error);
        /* this._snackBar.open('There was an issue fetching the expenses. Please try again later.', 'Close', {
          duration: 5000
        }); */
      }
    });
  }

}
