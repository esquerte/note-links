import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { Calendar } from '../models/calendar';
import { ApiService } from '../services/api.service';
import { InteractionService } from '../services/interaction.service'
import { CalendarCookieService } from '../services/calendar-cookie.service'
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-calendar-edit',
  templateUrl: './calendar-edit.component.html',
  styleUrls: ['./calendar-edit.component.scss']
})
export class CalendarEditComponent implements OnInit, OnDestroy {

  calendar: Calendar;
  private originalCalendar: Calendar;
  errorOccured: boolean;

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private router: Router,
    private apiService: ApiService,
    private interactionService: InteractionService,
    private cookieService: CalendarCookieService,
    private location: Location,
    private signalRService: SignalRService,
  ) { }

  ngOnInit() {
    this.interactionService.onStartEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      calendar => {
        this.calendar = calendar;
        this.originalCalendar = Object.assign({}, calendar);
      });
  }

  saveCalendar() {
    if (this.calendar.code) {
      this.updateCalendar();
    } else {
      this.createCalendar();
    }
  }

  private updateCalendar() {
    this.apiService.updateCalendar(this.calendar).subscribe(
      calendar => {
        this.cookieService.updateCalendarsCookie(calendar);
        this.interactionService.finishEditing(calendar);
        this.signalRService.change(calendar.code);
      },
      error => this.interactionService.handleError(error)
    );
  }

  private createCalendar() {
    this.apiService.createCalendar(this.calendar).subscribe(
      calendar => {
        this.router.navigate(['/calendars', calendar.code]);
      },
      error => this.interactionService.handleError(error)
    );
  }

  cancelEditing() {
    if (this.calendar.code) {
      this.interactionService.finishEditing(this.originalCalendar);
    } else {
      this.location.back();
    }
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
