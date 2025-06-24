import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';
import { MatIconModule } from '@angular/material/icon';
import { uniqueEmailValidator } from '../../../../_validators/uniqueEmailValidator';
import { AccountService } from '../../../../_services/account/account.service';

@Component({
  selector: 'et-signup-add-email',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './add-email.component.html',
  styleUrl: './add-email.component.css'
})
export class AddEmailComponent implements OnInit {
  private fb = inject(FormBuilder);
  _helperService = inject(HelperService);
  _accountService = inject(AccountService);
  @Output() emailAdded: EventEmitter<string> = new EventEmitter();
  @Input() email: string | null = null;

  emailForm: FormGroup = this.fb.group({
    email: [null, [Validators.required, Validators.email], [uniqueEmailValidator(this._accountService)]]
  });
  isFormSubmitted = false;


  ngOnInit(): void {
    if (this.email) {
      this.emailForm.patchValue({ email: this.email });
    }
  }

  onSubmit(): void {
    this.isFormSubmitted = true;
    if (this.emailForm.valid) {
      this.emailAdded.emit(this.emailForm.value.email);
    }
  }

  showValidationError(controlName: string): boolean | undefined {
    return this._helperService.showValidationError(this.emailForm, controlName, this.isFormSubmitted);
  }
}
