import { Component, Inject, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { AlertDialogData } from './alert-dialog-data';

@Component({
  selector: 'app-alert-dialog',
  imports: [
    MatButtonModule,
    MatDialogActions,
    MatDialogContent,
    MatDialogTitle
  ],
  templateUrl: './alert-dialog.component.html',
  styleUrl: './alert-dialog.component.css'
})
export class AlertDialogComponent {
  readonly dialogRef = inject(MatDialogRef<AlertDialogComponent>);
  title: string = '';
  message: string = '';
  okText: string = 'Ok';

  constructor(@Inject(MAT_DIALOG_DATA) private data: AlertDialogData) {
    if (data) {
      this.title = data.title;
      this.message = data.message || 'This is a message?';
      this.okText = data.OkButtonText || 'Ok';
    }
  }


  onConfirm() {
    this.dialogRef.close();
  }
}
