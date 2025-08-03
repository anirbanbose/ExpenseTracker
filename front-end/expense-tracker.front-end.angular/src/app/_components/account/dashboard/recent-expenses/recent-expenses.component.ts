import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { DashboardService } from '../../../../_services/dashboard/dashboard.service';
import { MatSnackBar } from '@angular/material/snack-bar';

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
  private _snackBar = inject(MatSnackBar);
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
        this._snackBar.open('There was an issue fetching the recent expenses. Please try again later.', 'Close', {
          duration: 5000
        });
      }
    });
  }


}
