import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PageTitleBarComponent } from '../common/page-title-bar/page-title-bar.component';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { RecentExpensesComponent } from './recent-expenses/recent-expenses.component';
import { ExpenseChartComponent } from './expense-chart/expense-chart.component';
import { DashboardService } from '../../../_services/dashboard/dashboard.service';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    PageTitleBarComponent,
    MatCardModule,
    RecentExpensesComponent,
    ExpenseChartComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  pageTitle = 'Dashboard';
  breadcrumbItems = [
    { label: 'Home', link: '/' },
    { label: 'Dashboard', link: null },
  ];
  private _dashboardService = inject(DashboardService);
  summaryData: any = {};


  ngOnInit(): void {
    this.getExpenseSummary();
  }


  trackById(index: number, expense: any) {
    return expense.id;
  }

  getExpenseSummary() {
    this._dashboardService.getExpenseSummary().subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.summaryData = response.value;
          console.log(this.summaryData);
        }
      },
      error: (error: any) => {
        console.log(error);
      }
    });
  }
}
