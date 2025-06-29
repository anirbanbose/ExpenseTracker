import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HelperService } from '../../../../../_helpers/helper-service/helper.service';
import { AuthService } from '../../../../../_services/auth/auth.service';
import { ExpenseCategoryService } from '../../../../../_services/expense-category/expense-category.service';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'et-add-edit-expense-category',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './add-edit-expense-category.component.html',
  styleUrl: './add-edit-expense-category.component.css'
})
export class AddEditExpenseCategoryComponent implements OnInit {
  private fb = inject(FormBuilder);
  private _helperService = inject(HelperService);
  private _authService = inject(AuthService);
  private _snackBar = inject(MatSnackBar);
  private _expenseCategoryService = inject(ExpenseCategoryService);
  @Input() expenseCategoryId: string | null = null;
  @Output() backToList: EventEmitter<void> = new EventEmitter<void>();

  expenseCategoryForm: FormGroup = this.fb.group({
    id: [null],
    name: [null, [Validators.required]],
  });
  isFormSubmitted: boolean = false;


  ngOnInit(): void {
    this.getExpenseCategoryById();
  }

  getExpenseCategoryById() {
    if (this.expenseCategoryId) {
      this._expenseCategoryService.getExpenseCategoryById(this.expenseCategoryId).subscribe({
        next: (data: any) => {
          if (data) {
            this.expenseCategoryForm.patchValue({
              id: data.id,
              name: data.name
            });
          }
        },
        error: (err: HttpErrorResponse) => {
          if (err.status == 401) {
            this._helperService.handle401Error(this._snackBar, this._authService);
          }
          else if (err.error?.errorMessage) {
            this._snackBar.open(err.error?.errorMessage, 'Close', {
              duration: 5000
            });
            this.backToList.emit();
          }
          else {
            this._snackBar.open('There was an issue in fetching the expense category. Please try again later.', 'Close', {
              duration: 5000
            });
            this.backToList.emit();
          }
        }
      });
    }
  }



  onSubmit() {
    this.isFormSubmitted = true;
    if (this.expenseCategoryForm.valid) {
      const formValue = this.expenseCategoryForm.value;
      if (this.expenseCategoryId && formValue.id) {
        this._expenseCategoryService.updateExpenseCategory(formValue).subscribe({
          next: (data: any) => {
            if (data) {
              this._snackBar.open(data.message, 'Close', {
                duration: 5000
              });
              this.backToList.emit();
            }
          },
          error: (err: HttpErrorResponse) => {
            if (err.status == 401) {
              this._helperService.handle401Error(this._snackBar, this._authService);
            }
            else if (err.error?.errorMessage) {
              this._snackBar.open(err.error?.errorMessage, 'Close', {
                duration: 5000
              });
            }
            else {
              this._snackBar.open('There was an issue in updating the expense category. Please try again later.', 'Close', {
                duration: 5000
              });
            }
          }
        });
      }
      else {
        this._expenseCategoryService.addExpenseCategory(formValue).subscribe({
          next: (data: any) => {
            if (data) {
              this._snackBar.open(data.message, 'Close', {
                duration: 5000
              });
              this.backToList.emit();
            }
          },
          error: (err: HttpErrorResponse) => {
            if (err.status == 401) {
              this._helperService.handle401Error(this._snackBar, this._authService);
            }
            else if (err.error?.errorMessage) {
              this._snackBar.open(err.error?.errorMessage, 'Close', {
                duration: 5000
              });
            }
            else {
              this._snackBar.open('There was an issue in adding the expense category. Please try again later.', 'Close', {
                duration: 5000
              });
            }
          }
        });
      }
    }
  }

  onCancel() {
    this.backToList.emit();
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.expenseCategoryForm, controlName, this.isFormSubmitted);
  }

}
