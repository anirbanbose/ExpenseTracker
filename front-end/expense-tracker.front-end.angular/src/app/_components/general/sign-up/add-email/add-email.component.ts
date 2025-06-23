import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';

@Component({
  selector: 'et-signup-add-email',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './add-email.component.html',
  styleUrl: './add-email.component.css'
})
export class AddEmailComponent {
  private fb = inject(FormBuilder);
  _helperService = inject(HelperService);

  emailForm: FormGroup = this.fb.group({
    email: [null, [Validators.required, Validators.email]]
  });
  isFormSubmitted = false;


  onSubmit(): void {
    this.isFormSubmitted = true;
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.emailForm, controlName, this.isFormSubmitted);
  }
}
