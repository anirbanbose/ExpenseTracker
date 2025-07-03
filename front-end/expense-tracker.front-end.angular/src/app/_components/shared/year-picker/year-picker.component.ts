import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { ReportService } from '../../../_services/report/report.service';


@Component({
  selector: 'et-year-picker',
  imports: [
    CommonModule,
    MatFormFieldModule,
    FormsModule,
    MatSelectModule
  ],
  templateUrl: './year-picker.component.html',
  styleUrl: './year-picker.component.css'
})
export class YearPickerComponent implements OnInit {
  @Input() maxDate: Date | null = null;
  @Input() minDate: Date | null = null;
  @Output() yearSelectedEvent: EventEmitter<number> = new EventEmitter<number>();
  minYear: number = 0;
  maxYear: number = 0;
  selectedYear: number = 0;
  years: { value: number, label: string }[] = [];
  private _reportService = inject(ReportService);

  ngOnInit(): void {
    this.getMinMaxDates();
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

  populateYears() {
    this.minYear = this.minDate ? this.minDate.getFullYear() : new Date().getFullYear();
    this.maxYear = this.maxDate ? this.maxDate.getFullYear() : new Date().getFullYear();
    for (let year = this.minYear; year <= this.maxYear; year++) {
      this.years.push({ value: year, label: year.toString() });
    }
    this.selectedYear = this.maxYear;
    this.yearSelected();
  }

  yearSelected() {
    this.yearSelectedEvent.emit(this.selectedYear);
  }
}
