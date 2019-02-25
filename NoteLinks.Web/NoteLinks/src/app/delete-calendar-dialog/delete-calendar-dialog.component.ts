import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';

export interface DialogData {
  calendarName: string;
}

@Component({
  selector: 'app-delete-calendar-dialog',
  templateUrl: './delete-calendar-dialog.component.html',
  styleUrls: ['./delete-calendar-dialog.component.scss']
})
export class DeleteCalendarDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<DeleteCalendarDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    public translate: TranslateService,
  ) {}

  onNoClick(): void {
    this.dialogRef.close();
  }

}
