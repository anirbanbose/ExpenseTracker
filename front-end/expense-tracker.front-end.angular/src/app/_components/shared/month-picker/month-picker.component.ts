import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { ReportService } from '../../../_services/report/report.service';


@Component({
  selector: 'et-month-picker',
  imports: [
    CommonModule,
    MatFormFieldModule,
    FormsModule,
    MatSelectModule
  ],
  templateUrl: './month-picker.component.html',
  styleUrl: './month-picker.component.css',
})
export class MonthPickerComponent implements OnInit {
  maxDate: Date | null = null;
  minDate: Date | null = null;
  @Output() monthSelectedEvent: EventEmitter<{ month: number, year: number }> = new EventEmitter<{ month: number, year: number }>();
  selectedMonth: number | null = null;
  selectedYear: number | null = null;
  minYear: number = 0;
  maxYear: number = 0;
  minMonth: number = -1;
  maxMonth: number = -1;
  private _reportService = inject(ReportService);
  hasError: boolean = true;
  monthOptions: { value: number, label: string }[] = [
    { value: 1, label: 'January' },
    { value: 2, label: 'February' },
    { value: 3, label: 'March' },
    { value: 4, label: 'April' },
    { value: 5, label: 'May' },
    { value: 6, label: 'June' },
    { value: 7, label: 'July' },
    { value: 8, label: 'August' },
    { value: 9, label: 'September' },
    { value: 10, label: 'October' },
    { value: 11, label: 'November' },
    { value: 12, label: 'December' }
  ];

  months: { value: number, label: string }[] = [];
  years: { value: number, label: string }[] = [];

  ngOnInit(): void {
    this.getMinMaxDates();
  }

  monthSelected() {
    if (this.selectedMonth && this.selectedYear) {
      this.monthSelectedEvent.emit({ month: this.selectedMonth, year: this.selectedYear });
    }
  }

  getMinMaxDates() {
    this._reportService.getMinMaxDates().subscribe({
      next: (data) => {
        this.maxDate = data.maxDate ? new Date(data.maxDate) : new Date();
        this.minDate = data.minDate ? new Date(data.minDate) : new Date();

        this.populateYears();
      },
      error: (error) => {
        console.error('Error fetching min/max dates:', error);
      }
    });
  }

  yearSelected() {
    this.months = [];
    this.selectedMonth = null;
    this.populateMonths();
  }

  populateYears() {
    this.years = [];
    this.minYear = this.minDate ? this.minDate.getFullYear() : new Date().getFullYear();
    this.maxYear = this.maxDate ? this.maxDate.getFullYear() : new Date().getFullYear();
    for (let year = this.minYear; year <= this.maxYear; year++) {
      this.years.push({ value: year, label: year.toString() });
    }
    this.selectedYear = this.maxYear;
    this.populateMonths();
  }

  populateMonths() {
    this.months = [];
    this.minMonth = this.minDate ? this.minDate.getMonth() : -1;
    this.maxMonth = this.maxDate ? this.maxDate.getMonth() : -1;

    let startMonth = 0; // Default start month is January (index 0)
    let endMonth = 11;  // Default end month is December (index 11)

    if (this.selectedYear === this.minYear) {
      startMonth = this.minMonth;
    }

    if (this.selectedYear === this.maxYear) {
      endMonth = this.maxMonth;
    }

    this.months = this.monthOptions.slice(startMonth, endMonth + 1);
  }

}
