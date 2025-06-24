import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'et-signup-registration-success',
  imports: [
    MatCardModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './registration-success.component.html',
  styleUrl: './registration-success.component.css'
})
export class RegistrationSuccessComponent {
  private _router = inject(Router);


  goToLogin(): void {
    this._router.navigate(['/sign-in']);
  }
}
