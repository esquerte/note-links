import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { Calendar } from '../models/calendar';
import { ApiService } from '../services/api.service';
import { CalendarService } from '../services/calendar.service'
import { CalendarCookieService } from '../services/calendar-cookie.service'

@Component({
  selector: 'app-calendar-edit',
  templateUrl: './calendar-edit.component.html',
  styleUrls: ['./calendar-edit.component.css']
})
export class CalendarEditComponent implements OnInit {

  calendar: Calendar;
  originalCalendar: Calendar;

  constructor(
    private router: Router,
    private apiService: ApiService,
    private calendarService: CalendarService,
    private cookieService: CalendarCookieService,
  ) {}

  ngOnInit() {
    this.calendarService.onStartEditing$.subscribe(
      calendar =>  { 
        if (calendar) {
          this.calendar = calendar;
          this.originalCalendar = Object.assign({}, calendar);
        }
    });    
  }

  saveCalendar(): void {
    if (this.calendar.code) {
      this.updateCalendar();
    } else {
      this.createCalendar();
    } 
  }

  private updateCalendar(): void {
    this.apiService.updateCalendar(this.calendar).subscribe(
      calendar => {
        this.cookieService.updateCalendarsCookie(calendar);
        this.calendarService.finishEditing(calendar);
    });     
  }

  private createCalendar(): void {
    this.apiService.createCalendar(this.calendar).subscribe(
      calendar => {           
        this.router.navigate(['/calendars', calendar.code]);      
    });  
  }

  cancelEditing(): void {
    this.calendarService.finishEditing(this.originalCalendar);
  }

}
