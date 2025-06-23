import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordMatchValidator(passwordField: string, confirmPasswordField: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
        const password = formGroup.get(passwordField);
        const confirmPassword = formGroup.get(confirmPasswordField);

        if (!password || !confirmPassword) {
            return null; // Skip if either field is missing
        }

        if (confirmPassword.errors && !confirmPassword.errors['passwordMismatch']) {
            return null; // Skip if other validators already found an error
        }

        const isMatching = password.value === confirmPassword.value;

        if (!isMatching) {
            confirmPassword.setErrors({ passwordMismatch: true });
        } else {
            confirmPassword.setErrors(null);
        }

        return null;
    };
}