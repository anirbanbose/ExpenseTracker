import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { CommonModule } from '@angular/common';
import { passwordMatchValidator } from '../../../../_validators/password-match-validator';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../../_services/auth/auth.service';
import { AccountService } from '../../../../_services/account/account.service';

@Component({
  selector: 'et-profile-change-password',
  imports: [
    CommonModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule
  ],
  templateUrl: './profile-change-password.component.html',
  styleUrl: './profile-change-password.component.css'
})
export class ProfileChangePasswordComponent {
  isFormSubmitted: boolean = false;
  errorMessage: string = '';
  private _snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  private _helperService = inject(HelperService);
  private _authService = inject(AuthService);
  private _accountService = inject(AccountService);

  passwordForm = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(18)]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator('newPassword', 'confirmPassword') });


  onChangePassword() {
    this.isFormSubmitted = true;
    this.errorMessage = '';
    if (this.passwordForm.valid) {
      const formValue = this.passwordForm.value;
      this._accountService.changePassword(formValue).subscribe({
        next: (data: any) => {
          if (data) {
            this._snackBar.open(data.message, 'Close', {
              duration: 5000
            });
          }
          this.clearForm();
        },
        error: (err: HttpErrorResponse) => {
          this.clearForm();
          if (err.status == 401) {
            this._helperService.handle401Error(this._snackBar, this._authService);
          }
          else if (err.error?.errorMessage) {
            this.errorMessage = err.error?.errorMessage;
          }
          else {
            this.errorMessage = 'There was an issue in changing the password. Please try again later.';
          }
        }
      });
    }
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.passwordForm, controlName, this.isFormSubmitted);
  }

  clearForm() {
    this._helperService.clearFrom(this.passwordForm);
    this.isFormSubmitted = false;
  }
}
