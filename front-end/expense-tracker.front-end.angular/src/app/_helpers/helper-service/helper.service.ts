import { inject, Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, throwError } from 'rxjs';
import { isDate, formatISO } from 'date-fns';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../_services/auth/auth.service';
@Injectable({
  providedIn: 'root'
})
export class HelperService {
  private _router = inject(Router);
  private callMethodSource = new Subject<void>();
  callMethod$ = this.callMethodSource.asObservable();

  constructor() { }

  triggerMethod() {
    this.callMethodSource.next();
  }

  capitalizeFirstLetter(str: string) {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }

  navigateWithSearchParams(queryParams: any) {
    this._router.navigate([], {
      queryParams: queryParams,
      queryParamsHandling: 'merge'
    });
  }

  goToReturnUrl(returnUrl: string | null) {
    if (returnUrl) {
      this._router.navigateByUrl(returnUrl, { replaceUrl: true });
    }
  }

  navigateToUrlWithSearchParams(url: string, queryParams: any) {
    this._router.navigate([`${url}`], {
      queryParams: queryParams,
      queryParamsHandling: 'merge'
    });
  }

  getErrorMessages(errorObject: any): string[] {
    let errorMessages: string[] = [];
    let error = errorObject.error;
    let propNames = Object.keys(error);
    for (let propName of propNames) {
      errorMessages.push(error[propName][0]);
    }
    return errorMessages;
  }

  handleError(error: any) {
    let errorMessage = '';
    if (error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.message}`;
    } else {
      errorMessage = `${error.error?.errorMessage}`;
    }
    return throwError(() => errorMessage);
  }

  handle401Error(snackBar: MatSnackBar, authService: AuthService) {
    snackBar.open("You are not authorized to see this page.", 'Close', {
      duration: 5000
    });
    authService.logout();
  }

  getNumberOrZero(value: string | null): number {
    if (value && this.isNumber(value)) {
      let numVal = Number(value);
      if (!isNaN(numVal)) {
        return numVal;
      }
    }
    return 0;
  }
  getIntegerOrZero(value: string | null): number {
    if (value && this.isNumber(value)) {
      let numVal = parseInt(value);
      if (!isNaN(numVal)) {
        return numVal;
      }
    }
    return 0;
  }

  isNumber(value: string | null) {
    return !isNaN(Number(value));
  }

  getSortDirection(value: string | null | undefined) {
    if (value && (value === 'asc' || value === 'desc')) {
      return value
    }
    return 'asc';
  }

  showValidationError(form: FormGroup<any>, controlName: string, isSubmitted: boolean): boolean | undefined {
    if (!controlName || controlName.trim() == '' || form.get(controlName) == null) {
      return false;
    }
    return form.get(controlName)?.invalid && (form.get(controlName)?.touched || isSubmitted);
  }

  getDateOnly(date: Date | null): string | null {
    if (date) {
      if (isDate(date)) {
        return formatISO(date).split('T')[0];
      }
      else if (typeof date == 'string') {
        return date;
      }
    }
    return formatISO(new Date()).split('T')[0]; // Return today's date in YYYY-MM-DD format
  }

  clearFrom(form: FormGroup<any>) {
    form.reset();
    form.markAsPristine();
    form.markAsUntouched();
    Object.keys(form.controls).forEach((key: string) => {
      const control: any = form.get(key);
      control?.setErrors(null);
    });
  }
}
