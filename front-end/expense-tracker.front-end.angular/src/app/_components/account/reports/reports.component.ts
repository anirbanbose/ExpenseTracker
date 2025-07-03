import { Component, inject, OnInit } from '@angular/core';
import { PageTitleBarComponent } from '../common/page-title-bar/page-title-bar.component';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule, } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MonthPickerComponent } from '../../shared/month-picker/month-picker.component';
import { ReportService } from '../../../_services/report/report.service';
import { YearPickerComponent } from '../../shared/year-picker/year-picker.component';
import { MatSnackBar } from '@angular/material/snack-bar';

enum ReportType {
  Monthly = 1,
  Yearly = 2,
}

enum ViewMode {
  Excel = 1,
  Pdf = 2,
  Screen = 3,
}


@Component({
  selector: 'app-reports',
  imports: [
    CommonModule,
    PageTitleBarComponent,
    MatCardModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    FormsModule,
    MatInputModule,
    MonthPickerComponent,
    YearPickerComponent
  ],
  providers: [
  ],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css',
})
export class ReportsComponent implements OnInit {
  pageTitle = 'Reports';
  breadcrumbItems = [
    { label: 'Dashboard', link: '/account/dashboard' },
    { label: 'Reports', link: '/account/reports' },
  ];
  selectedReportType = ReportType.Monthly;
  selectedViewMode = ViewMode.Excel;
  selectedDate: any | null = null;
  maxDate: Date = new Date();
  minDate: Date = new Date();
  reportData: any[] | null = null;
  private _reportService = inject(ReportService);
  private _snackBar = inject(MatSnackBar);

  ngOnInit(): void {
  }


  reportTypeChanged() {
    this.selectedDate = null;
  }
  viewModeChanged() {
  }

  loadReport() {
    debugger;
    if (this.selectedDate) {
      switch (this.selectedViewMode) {
        case ViewMode.Excel:
          this.downloadReport();
          break;
        case ViewMode.Pdf:
          this.downloadReport();
          break;
        default:
          this.showOnScreenReport();
          break;
      }
    }
  }


  dateRangeSelected(event: any) {
    console.log('Date Range Selected:', event);
    this.selectedDate = event;
  }


  showOnScreenReport() {
  }


  downloadReport() {
    debugger;
    let year: number, month: number | null, reportType: 1 | 2;

    if (this.selectedReportType === ReportType.Monthly && this.selectedDate) {
      year = this.selectedDate.year;
      month = this.selectedDate.month;
      reportType = 1;
    }
    else {
      year = this.selectedDate;
      month = null;
      reportType = 2;
    }
    let reportFormat: 1 | 2 = this.selectedViewMode === ViewMode.Pdf ? 2 : 1;
    this._reportService.downloadExpenseReport(reportType, year, month, reportFormat).subscribe({
      next: (blob) => {
        const extension = this.selectedViewMode === ViewMode.Pdf ? 'pdf' : 'xlsx';
        const fileName = `Expense Export.${extension}`;

        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
        window.URL.revokeObjectURL(link.href); // Clean up
      },
      error: (err) => {
        this._snackBar.open('Report failed. Please try again later.', 'Close', {
          duration: 5000
        })
      }
    });
  }


}
