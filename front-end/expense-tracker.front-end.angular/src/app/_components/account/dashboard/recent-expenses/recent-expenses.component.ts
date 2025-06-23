import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { DashboardService } from '../../../../_services/dashboard/dashboard.service';

@Component({
  selector: 'et-dashboard-recent-expenses',
  imports: [
    CommonModule,
    MatTableModule
  ],
  templateUrl: './recent-expenses.component.html',
  styleUrl: './recent-expenses.component.css'
})
export class RecentExpensesComponent {
  private _dashboardService = inject(DashboardService);
  recentExpenses = new MatTableDataSource<any>([]);
  recordCount: number = 5;

  ngOnInit(): void {
    this.getRecentExpenses();
  }


  getRecentExpenses() {
    this._dashboardService.getRecentExpenses(this.recordCount).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.recentExpenses = response.value;
        }
      },
      error: (error: any) => {
        console.log(error);
      }
    });
  }


}
