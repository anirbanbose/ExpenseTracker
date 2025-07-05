import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Observable } from 'rxjs/internal/Observable';
import { ExpenseCategoryService } from '../../../../../_services/expense-category/expense-category.service';
import { map, startWith } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { CurrencyService } from '../../../../../_services/currency/currency.service';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'et-expense-advanced-search',
  imports: [
    CommonModule,
    MatFormField,
    MatInputModule,
    MatAutocompleteModule,
    MatDatepickerModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatSelectModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './expense-advanced-search.component.html',
  styleUrl: './expense-advanced-search.component.css'
})
export class ExpenseAdvancedSearchComponent implements OnInit {
  private activatedRoute = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  @Output() advancedSearch: EventEmitter<any> = new EventEmitter();
  filteredCategories!: Observable<any[]>;
  expenseCategories: any[] = [];
  private _expenseCategoryService = inject(ExpenseCategoryService);

  searchForm: FormGroup = this.fb.group({
    searchText: [''],
    category: [null],
    startDate: [null],
    endDate: [null]
  });
  expenseCategoryId: any = null;;

  constructor() {
    this.activatedRoute.queryParamMap.subscribe(params => {
      this.searchForm.patchValue({
        searchText: params.get('q'),
        startDate: params.get('sd'),
        endDate: params.get('ed')
      });
      this.expenseCategoryId = params.get('cid');
    });
  }

  ngOnInit(): void {
    this.getExpenseCategories();
    this.searchForm.valueChanges.pipe(
      startWith(null),
      map((value: any) => {
        console.log(value);
      }),
    );
  }

  get expenseCategoryControl(): any {
    return this.searchForm.get('category');
  }

  private _filterExpenseCategories(value: any): any[] {
    let filterValue = '';
    if (value) {
      if (typeof value == 'object') {
        filterValue = value?.name?.toLowerCase();
      }
      else if (typeof value == 'string') {
        filterValue = value?.toLowerCase();
      }
    }

    let retVal = this.expenseCategories.filter(option => option?.name.toLowerCase().includes(filterValue));
    return retVal;
  }

  getExpenseCategories(): void {
    this._expenseCategoryService.getExpenseCategories().subscribe({
      next: (data: any) => {
        if (data) {
          this.expenseCategories = data;
          if (this.expenseCategoryId) {
            let expCategory = this.expenseCategories.find(x => x.id == this.expenseCategoryId);
            if (expCategory) {
              this.searchForm.patchValue({
                category: expCategory
              });
            }
          }
          this.filteredCategories = this.expenseCategoryControl.valueChanges.pipe(
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

  displayFn(obj: any): string {
    return obj ? obj.name : '';
  }


  resetSearch(): void {
    this.searchForm.patchValue({
      searchText: '',
      category: null,
      startDate: null,
      endDate: null
    });
    this.advancedSearch.emit({});
  }

  onSearch(): void {
    this.advancedSearch.emit({
      searchText: this.searchForm.value.searchText,
      category: this.searchForm.value.category?.id,
      startDate: this.searchForm.value.startDate,
      endDate: this.searchForm.value.endDate,
    });
  }
}
