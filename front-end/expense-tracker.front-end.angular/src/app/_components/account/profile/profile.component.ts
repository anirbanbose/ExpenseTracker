import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { PageTitleBarComponent } from '../common/page-title-bar/page-title-bar.component';
import { MatCardModule } from '@angular/material/card';
import { ProfileChangePasswordComponent } from './profile-change-password/profile-change-password.component';
import { ProfileGeneralInfoComponent } from './profile-general-info/profile-general-info.component';

@Component({
  selector: 'app-profile',
  imports: [
    CommonModule,
    PageTitleBarComponent,
    MatCardModule,
    ProfileChangePasswordComponent,
    ProfileGeneralInfoComponent
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  pageTitle = 'Profile';
  breadcrumbItems = [
    { label: 'Dashboard', link: '/account/dashboard' },
    { label: 'Profile', link: '/account/profile' },
  ];
}
