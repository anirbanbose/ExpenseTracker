import { Component, inject, OnInit } from '@angular/core';
import { ChartConfiguration, ChartData } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { DashboardService } from '../../../../_services/dashboard/dashboard.service';

@Component({
  selector: 'et-dashboard-expense-chart',
  imports: [
    BaseChartDirective
  ],
  templateUrl: './expense-chart.component.html',
  styleUrl: './expense-chart.component.css'
})
export class ExpenseChartComponent implements OnInit {
  private _dashboardService = inject(DashboardService);

  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    scales: {
      x: {},
      y: {
        min: 10,
      },
    },
    plugins: {
      legend: {
        display: true,
      },

    },
  };
  public barChartType = 'bar' as const;

  public barChartData: ChartData<'bar'> = {
    labels: [],
    datasets: []
  };

  ngOnInit(): void {
    this.getChartData();
  }

  getChartData() {
    this._dashboardService.getSpendingChartData().subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.barChartData = response.value;
        }
      },
      error: (error: any) => {
        console.log(error);
      }
    });
  }
}
