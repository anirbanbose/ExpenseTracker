import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-expense-export-dialog',
  imports: [
    MatButtonModule,
    MatRadioModule,
    MatDialogActions,
    MatDialogContent,
    MatDialogTitle,
    FormsModule
  ],
  templateUrl: './expense-export-dialog.component.html',
  styleUrl: './expense-export-dialog.component.css'
})
export class ExpenseExportDialogComponent {
  readonly dialogRef = inject(MatDialogRef<ExpenseExportDialogComponent>);
  title: string = '';
  exportFormat: string = "1";


  onDismiss(): void {
    this.dialogRef.close();
  }

  onConfirm(): void {
    this.dialogRef.close(this.exportFormat);
  }
}
