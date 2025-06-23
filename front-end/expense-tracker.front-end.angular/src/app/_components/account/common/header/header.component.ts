import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { fadeDropdown } from '../../../../_helpers/animations';
import { AuthService } from '../../../../_services/auth/auth.service';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { HelperService } from '../../../../_helpers/helper-service/helper.service';

@Component({
  selector: 'et-account-header',
  imports: [
    RouterModule,
    CommonModule,
    MatToolbarModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  animations: [fadeDropdown],
})
export class HeaderComponent implements OnInit {
  userFullName: string = '';
  private _authService = inject(AuthService);
  private _helperService = inject(HelperService);
  private _router = inject(Router);
  private subscription!: Subscription;
  ngOnInit(): void {
    this.setUserName();
    this.subscription = this._helperService.callMethod$.subscribe(() => {
      this.setUserName();
    });
  }

  logout(): void {
    this._authService.logout().subscribe(() => {
      this.userFullName = '';
      this._router.createUrlTree(['/sign-in'])
      this._router.navigate(['/logout'], { replaceUrl: true });
    });;
  }

  private setUserName() {
    let user = this._authService.getLoggedInUser();
    if (user) {
      this.userFullName = user.fullName;
    }
    else {
      this.logout();
    }
  }

}
