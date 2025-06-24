import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { catchError, map, debounceTime, switchMap } from 'rxjs/operators';


import { AccountService } from './../_services/account/account.service';

export function uniqueEmailValidator(accountService: AccountService): AsyncValidatorFn {
    return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {

        if (!control.value) {
            return of(null);
        }

        return of(control.value).pipe(
            debounceTime(500),
            switchMap(email => accountService.emailAvailable(email)),
            map(isAvailable => (!isAvailable ? { uniqueEmail: true } : null)),
            catchError(() => of(null))
        );
    };
}