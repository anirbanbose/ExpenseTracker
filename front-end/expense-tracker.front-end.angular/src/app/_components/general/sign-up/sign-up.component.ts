import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { AddEmailComponent } from './add-email/add-email.component';
import { AddPasswordComponent } from './add-password/add-password.component';
import { RegistrationSuccessComponent } from './registration-success/registration-success.component';
import { AddDetailsComponent } from './add-details/add-details.component';
import { AccountService } from '../../../_services/account/account.service';
import { HttpErrorResponse } from '@angular/common/http';

enum RegistrationProcess {
  AddEmail = 1,
  AddPassword,
  AddDetails,
  RegistrationSuccess
}

@Component({
  selector: 'app-sign-up',
  imports: [
    CommonModule,
    MatCardModule,
    AddEmailComponent,
    AddPasswordComponent,
    AddDetailsComponent,
    RegistrationSuccessComponent
  ],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {
  pageTitle: string = '';
  errorMessage: string = ''
  registrationProcess: RegistrationProcess = RegistrationProcess.AddEmail;
  _accountService = inject(AccountService);

  signupModel: any = {
    email: null,
    password: null,
    firstName: null,
    lastName: null,
    middleName: null,
    preferredCurrencyId: null
  }

  onEmailAdded(event: any): void {
    this.registrationProcess = RegistrationProcess.AddPassword;
    this.signupModel.email = event;
  }

  onPasswordAdded(event: any): void {
    this.registrationProcess = RegistrationProcess.AddDetails;
    this.signupModel.password = event;
  }

  onDetailsAdded(event: any): void {
    this.getDetailsData(event);
    this.registerUser();
  }

  getDetailsData(model: any): void {
    this.signupModel.firstName = model.firstName;
    this.signupModel.lastName = model.lastName;
    this.signupModel.middleName = model.middleName;
    this.signupModel.preferredCurrencyId = model.preferredCurrency;
  }

  onBackButtonClicked(): void {
    if (this.registrationProcess == RegistrationProcess.AddDetails) {
      this.signupModel.preferredCurrency = null;
      this.registrationProcess = RegistrationProcess.AddPassword;
    } else if (this.registrationProcess == RegistrationProcess.AddPassword) {
      this.signupModel.password = null;
      this.registrationProcess = RegistrationProcess.AddEmail;
    }
  }

  registerUser(): void {
    this._accountService.register(this.signupModel).subscribe({
      next: (data: any) => {
        if (data) {
          this.registrationProcess = RegistrationProcess.RegistrationSuccess;
        }
      },
      error: (err: HttpErrorResponse) => {
        if (err.error?.errorMessage) {
          this.errorMessage = err.error.errorMessage;
        }
        else {
          this.errorMessage = "Registration failed. Please try again later.";
        }
      }
    });
  }

  getPageTitle(): string {
    switch (this.registrationProcess) {
      case RegistrationProcess.AddEmail:
        return 'Add your email address';
      case RegistrationProcess.AddPassword:
        return 'Add Password';
      case RegistrationProcess.AddDetails:
        return 'Add Details';
      default:
        return '';
    }
  }
}
