import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordMatchValidator } from '../../../../_validators/password-match-validator';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'et-signup-add-password',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './add-password.component.html',
  styleUrl: './add-password.component.css'
})
export class AddPasswordComponent {
  _helperService = inject(HelperService);
  private fb = inject(FormBuilder);
  @Output() passwordAdded: EventEmitter<string> = new EventEmitter();
  @Output() onBack: EventEmitter<void> = new EventEmitter();

  passwordForm: FormGroup = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(18)]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator('password', 'confirmPassword') });
  isFormSubmitted = false;

  onSubmit(): void {
    this.isFormSubmitted = true;
    if (this.passwordForm.valid) {
      this.passwordAdded.emit(this.passwordForm.value.password);
    }
  }

  goBack() {
    this._helperService.clearFrom(this.passwordForm);
    this.onBack.emit();
  }


  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.passwordForm, controlName, this.isFormSubmitted);
  }

}
