import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { AddEmailComponent } from './add-email/add-email.component';

@Component({
  selector: 'app-sign-up',
  imports: [
    CommonModule,
    MatCardModule,
    AddEmailComponent
  ],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {
  errorMessage: string = ''
}
