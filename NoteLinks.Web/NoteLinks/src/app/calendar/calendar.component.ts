import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatDialog, MatSnackBar } from '@angular/material';

import { Calendar } from '../models/calendar';
import { ApiService } from '../services/api.service'
import { CalendarCookieService } from '../services/calendar-cookie.service'
import { InteractionService } from '../services/interaction.service'
import { Note } from '../models/note';
import { SignalRService } from '../services/signal-r.service';
import { DeleteCalendarDialogComponent } from '../delete-calendar-dialog/delete-calendar-dialog.component';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit, OnDestroy {

  calendar: Calendar;
  calendarIsOnEditing: boolean;

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private cookieService: CalendarCookieService,
    private interactionService: InteractionService,
    private signalRService: SignalRService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar,
    public translate: TranslateService,
  ) {
    // https://stackoverflow.com/questions/41678356/router-navigate-does-not-call-ngoninit-when-same-page
    route.params.subscribe(() => {
      this.calendar = null;
      this.getCalendar()
    });
  }

  ngOnInit() {
    this.signalRService.onUpdate$.pipe(takeUntil(this.unsubscribe)).subscribe(
      calendaCode => {
        if (this.calendar.code == calendaCode) {
          this.getCalendar();
          this.showUpdateMessage();
        }
      });
    this.interactionService.onEditCalendar$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.editCalendar()
    );
    this.interactionService.onFinishEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      (calendar: Calendar) => {
        Object.assign(this.calendar, calendar);
        this.calendarIsOnEditing = false;
      });
    this.interactionService.onDeleteCalendar$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.deleteCalendar()
    );
  }

  getCalendar(): void {
    const code = this.route.snapshot.paramMap.get('code');
    if (code) {
      this.apiService.getCalendar(code).subscribe(
        calendar => {
          this.cookieService.updateCalendarsCookie(calendar);
          this.calendar = calendar;
        },
        () => this.router.navigate(["/"])
      );
    } else {
      this.calendar = new Calendar();
      this.editCalendar();
    }
  }

  private editCalendar() {
    this.interactionService.startEditing(this.calendar);
    this.calendarIsOnEditing = true;
  }

  private deleteCalendar() {

    const dialogRef = this.dialog.open(DeleteCalendarDialogComponent, {
      width: '350px',
      data: { calendarName: this.calendar.name }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.deleteCalendar(this.calendar.code).subscribe(
          () => {
            this.cookieService.deleteCalendarFromCookie(this.calendar.code);
            this.router.navigate(["/"]);
          },
          error => this.interactionService.handleError(error)
        );
      }
    });

  }

  createNote() {
    this.interactionService.selectNote(this.calendar.code, new Note());
  }

  showUpdateMessage() {    
    this.snackBar.open(this.translate.instant("calendar.updatedByAnotherUserMessage"), "", {
      duration: 5000,
      panelClass: ['update-snack-bar']
    });
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
