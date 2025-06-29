import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../_services/auth/auth.service';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HelperService } from '../../../_helpers/helper-service/helper.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'app-sign-in',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
  ],
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css'
})
export class SignInComponent implements OnInit {
  errorMessage: string = '';
  returnUrl: string = '';
  private fb = inject(FormBuilder);
  private _authService = inject(AuthService);
  private _helperService = inject(HelperService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loginForm: FormGroup = this.fb.group({
    email: [null, [Validators.required, Validators.email]],
    password: [null, Validators.required],
    rememberMe: [null]
  });
  isFormSubmitted = false;

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '';
    this._authService.checkAuthStatus().subscribe(isAuthenticated => {
      if (isAuthenticated) {
        if (this.returnUrl && this.returnUrl.trim() != '') {
          this.router.navigate([this.returnUrl]);
        }
        else {
          this.router.navigate(['/account/dashboard']);
        }
      }
    });
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.isFormSubmitted = true;
    if (this.loginForm.invalid) {
      return;
    }
    let _loginModel: any = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value,
      rememberMe: false,
    };
    this._authService.login(_loginModel).subscribe({
      next: (data: any) => {
        if (data) {
          if (data?.isLoggedIn) {
            if (this.returnUrl && this.returnUrl.trim() != '') {
              //window.location.href = this.returnUrl;
              this.router.navigateByUrl(this.returnUrl, { replaceUrl: true });
            }
            else {
              this.router.navigate(['/account/dashboard']);
            }
          }
        }
        else {
          this.errorMessage = "Invalid login attempt. Please try again later.";
        }
      },
      error: (err: HttpErrorResponse) => {
        if (err.status == 401) {
          this.errorMessage = err.error?.errorMessage;
        }
        else if (err.error?.errorMessage) {
          this.errorMessage = err.error?.errorMessage;
        }
        else {
          this.errorMessage = "Invalid login attempt. Please try again later";
        }
      }
    });
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.loginForm, controlName, this.isFormSubmitted);
  }
}
