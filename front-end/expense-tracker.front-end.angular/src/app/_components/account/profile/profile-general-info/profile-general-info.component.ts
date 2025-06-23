import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { CommonModule } from '@angular/common';
import { ProfileService } from '../../../../_services/profile/profile.service';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../../_services/auth/auth.service';

@Component({
  selector: 'et-profile-general-info',
  imports: [
    CommonModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule
  ],
  templateUrl: './profile-general-info.component.html',
  styleUrl: './profile-general-info.component.css'
})
export class ProfileGeneralInfoComponent implements OnInit {

  private _snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  private _helperService = inject(HelperService);
  private _profileService = inject(ProfileService);
  private _authService = inject(AuthService);
  isFormSubmitted = false;


  profileForm: FormGroup = this.fb.group({
    firstName: [null, [Validators.required, Validators.maxLength(100)]],
    lastName: [null, [Validators.required, Validators.maxLength(100)]],
    middleName: [null, Validators.maxLength(100)],
    email: [{ value: null, disabled: true }, [Validators.required, Validators.maxLength(250)]]
  });

  ngOnInit(): void {
    this.getProfileData();
  }

  onSaveProfile() {
    this.isFormSubmitted = true;
    if (this.profileForm.valid) {
      const formValue = this.profileForm.value;
      this._profileService.updateProfile(formValue).subscribe({
        next: (data: any) => {
          if (data) {
            this._snackBar.open(data.message, 'Close', {
              duration: 5000
            });
            this.getLoggedInUser();
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
            this._snackBar.open('There was an issue in updating the profile. Please try again later.', 'Close', {
              duration: 5000
            });
          }
        }
      });
    }
  }

  getProfileData() {
    this._profileService.getUserProfile().subscribe({
      next: (data: any) => {
        console.log(data);
        if (data && data.isSuccess) {
          console.log(data);
          this.profileForm.patchValue({
            email: data.value.email,
            firstName: data.value.firstName,
            lastName: data.value.lastName,
            middleName: data.value.middleName,
          });
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
          this._snackBar.open('There was an issue fetching the profile record. Please try again later.', 'Close', {
            duration: 5000
          });
        }
      }
    });
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.profileForm, controlName, this.isFormSubmitted);
  }

  getLoggedInUser(): any {
    this._profileService.getLoggedinUser().subscribe({
      next: (data: any) => {
        if (data) {
          this._authService.setLoggedinUser(data);
          this._helperService.triggerMethod();
        }
      },
      error: (err: HttpErrorResponse) => {
        this._snackBar.open('There was an issue in updating the profile. Please try again later.', 'Close', {
          duration: 5000
        });
      }
    });
  }
}
