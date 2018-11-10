import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Calendar } from '../models/calendar';
import { CalendarService } from '../services/calendar.service'
import { CalendarCookieService } from '../services/calendar-cookie.service'

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {

  calendar: Calendar;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private calendarService: CalendarService,
    private cookieService: CalendarCookieService
  ) { 
    // https://stackoverflow.com/questions/41678356/router-navigate-does-not-call-ngoninit-when-same-page
    route.params.subscribe(() => {    
      this.calendar = null;  
      this.getCalendar()
    });
  }

  ngOnInit() {}

  getCalendar(): void {
    const code = this.route.snapshot.paramMap.get('code');    
    this.calendarService.getCalendar(code).subscribe(
      calendar => {
        this.cookieService.updateCalendarsCookie(calendar);
        this.calendar = calendar;        
      }
    );
  }

  deleteCalendar(): void {
    this.calendarService.deleteCalendar(this.calendar.code).subscribe(
      () => {
        this.cookieService.deleteCalendarFromCookie(this.calendar.code);
        this.router.navigate(["/"]);    
      }
    );
  } 
  
}
