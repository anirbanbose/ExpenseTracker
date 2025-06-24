import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { CurrencyService } from '../../../../_services/currency/currency.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../../_services/auth/auth.service';
import { first } from 'rxjs';

@Component({
  selector: 'et-signup-add-details',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './add-details.component.html',
  styleUrl: './add-details.component.css'
})
export class AddDetailsComponent implements OnInit {
  private fb = inject(FormBuilder);
  _helperService = inject(HelperService);
  _currencyService = inject(CurrencyService);
  _authService = inject(AuthService);
  private _snackBar = inject(MatSnackBar);

  @Output() detailsAdded: EventEmitter<any> = new EventEmitter();
  @Output() onBack: EventEmitter<void> = new EventEmitter();
  detailsForm: FormGroup = this.fb.group({
    firstName: [null, [Validators.required, Validators.maxLength(100)]],
    lastName: [null, [Validators.required, Validators.maxLength(100)]],
    middleName: [null, [Validators.maxLength(100)]],
    preferredCurrency: [null, [Validators.required]]
  });
  isFormSubmitted = false;
  currencies: any[] = [];


  ngOnInit(): void {
    this.getCurrencies();
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
          this._helperService.handle401Error(this._snackBar, this._authService);
        }
        else if (err.error?.errorMessage) {

        }
        else {

        }
      }
    });
  }

  onSubmit(): void {
    this.isFormSubmitted = true;
    if (this.detailsForm.valid) {
      this.detailsAdded.emit(this.detailsForm.value);
    }
  }

  goBack() {
    this._helperService.clearFrom(this.detailsForm);
    this.onBack.emit();
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.detailsForm, controlName, this.isFormSubmitted);
  }
}
