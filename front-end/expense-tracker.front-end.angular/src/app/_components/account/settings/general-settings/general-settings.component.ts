import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { AuthService } from '../../../../_services/auth/auth.service';
import { CommonModule } from '@angular/common';
import { CurrencyService } from '../../../../_services/currency/currency.service';
import { HttpErrorResponse } from '@angular/common/http';
import { SettingsService } from '../../../../_services/settings/settings.service';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'et-general-settings',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatCardModule,
    MatCheckboxModule
  ],
  templateUrl: './general-settings.component.html',
  styleUrl: './general-settings.component.css'
})
export class GeneralSettingsComponent implements OnInit {
  private _snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  private _helperService = inject(HelperService);
  private _authService = inject(AuthService);
  private _currencyService = inject(CurrencyService);
  private _settingsService = inject(SettingsService);
  isFormSubmitted = false;
  currencies: any[] = [];

  settingsForm: FormGroup = this.fb.group({
    preferredCurrencyId: [null, [Validators.required]],
    enableMonthlyExpenseReportMail: [false],
    enableDailyExpenseReportMail: [false]
  });


  ngOnInit(): void {
    this.getCurrencies();
    this.getSettingsData();
  }

  getSettingsData() {
    this._settingsService.getUserPreference().subscribe({
      next: (data: any) => {
        if (data && data.isSuccess) {
          this.settingsForm.patchValue({
            preferredCurrencyId: data.value?.preferredCurrencyId,
            enableMonthlyExpenseReportMail: data.value?.enableMonthlyExpenseReportMail,
            enableDailyExpenseReportMail: data.value?.enableDailyExpenseReportMail
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
        }
        else {
          this._snackBar.open('There was an issue in fetching the user settings. ', 'Close', {
            duration: 5000
          });
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
        if (err.error?.errorMessage) {
          this._snackBar.open(err.error?.errorMessage, 'Close', {
            duration: 5000
          });
        }
        else {
          this._snackBar.open('There was an issue in fetching currencies. ', 'Close', {
            duration: 5000
          });
        }
      }
    });
  }

  saveSettings() {
    this.isFormSubmitted = true;
    if (this.settingsForm.valid) {
      const formValue = this.settingsForm.value;
      this._settingsService.saveUserPreference(formValue).subscribe({
        next: (data: any) => {
          if (data) {
            this._snackBar.open(data.message, 'Close', {
              duration: 5000
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
          }
          else {
            this._snackBar.open('There was an issue in saving the settings. Please try again later.', 'Close', {
              duration: 5000
            });
          }
        }
      });
    }
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.settingsForm, controlName, this.isFormSubmitted);
  }

}
