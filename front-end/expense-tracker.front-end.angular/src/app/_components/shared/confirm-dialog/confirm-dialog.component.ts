import { Component, Inject, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { ConfirmDialogData } from './confirm-dialog-data';

@Component({
  selector: 'app-confirm-dialog',
  imports: [
    MatButtonModule,
    MatDialogActions,
    MatDialogContent,
    MatDialogTitle
  ],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.css'
})
export class ConfirmDialogComponent {
  readonly dialogRef = inject(MatDialogRef<ConfirmDialogComponent>);
  title: string = '';
  message: string = '';
  okText: string = '';
  cancelText: string = '';

  constructor(@Inject(MAT_DIALOG_DATA) private data: ConfirmDialogData) {
    if (data) {
      this.title = data.title;
      this.message = data.message || 'Are you sure you want to perform this action?';
      this.okText = data.OkButtonText || 'Yes';
      this.cancelText = data.cancelButtonText || 'Cancel';
    }
  }

  onDismiss() {
    this.dialogRef.close();
  }

  onConfirm() {
    this.dialogRef.close(true);
  }
}


