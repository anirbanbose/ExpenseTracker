import { Component } from '@angular/core';
import { PageTitleBarComponent } from '../common/page-title-bar/page-title-bar.component';
import { GeneralSettingsComponent } from './general-settings/general-settings.component';
import { ExpenseCategoriesComponent } from './expense-categories/expense-categories.component';

@Component({
  selector: 'app-settings',
  imports: [
    PageTitleBarComponent,
    GeneralSettingsComponent,
    ExpenseCategoriesComponent
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css'
})
export class SettingsComponent {
  pageTitle = 'Settings';
  breadcrumbItems = [
    { label: 'Dashboard', link: '/account/dashboard' },
    { label: 'Settings', link: '/account/settings' },
  ];
}
