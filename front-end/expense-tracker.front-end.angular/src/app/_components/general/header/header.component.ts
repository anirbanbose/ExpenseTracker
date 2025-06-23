import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { fadeDropdown } from '../../../_helpers/animations';
import { AuthService } from '../../../_services/auth/auth.service';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'et-general-header',
  imports: [
    RouterModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    CommonModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  animations: [fadeDropdown],
})
export class HeaderComponent implements OnInit {
  userFullName: string = '';
  isAuthenticated: boolean = false;
  private _authService = inject(AuthService);
  private _router = inject(Router);

  ngOnInit(): void {
    if (this.checkRouteContainsString('/logout')) {
      this._authService.logout().subscribe(() => {
        this.isAuthenticated = false;
        this.userFullName = '';
        this._router.createUrlTree(['/sign-in'])
      });
      return;
    }
    this._authService.checkAuthStatus().subscribe(isAuthenticated => {
      this.isAuthenticated = isAuthenticated;
    });
    let user = this._authService.getLoggedInUser();
    if (user) {
      this.userFullName = user.fullName;
    }
  }

  logout(): void {
    this._authService.logout().subscribe(() => {
      this.isAuthenticated = false;
      this.userFullName = '';
      this._router.createUrlTree(['/sign-in'])
      this._router.navigate(['/logout'], { replaceUrl: true });
    });
  }

  checkRouteContainsString(searchString: string): boolean {
    return this._router.url.includes(searchString);
  }
}
