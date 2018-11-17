import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Calendar } from '../models/calendar';
import { ApiService } from '../services/api.service'
import { CalendarCookieService } from '../services/calendar-cookie.service'
import { CalendarService } from '../services/calendar.service'
import { Note } from '../models/note';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {

  calendar: Calendar;
  calendarIsOnEditing: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private cookieService: CalendarCookieService,
    private calendarService: CalendarService,
  ) { 
    // https://stackoverflow.com/questions/41678356/router-navigate-does-not-call-ngoninit-when-same-page
    route.params.subscribe(() => {    
      this.calendar = null;  
      this.getCalendar()
    });
  }

  ngOnInit() {
    this.calendarService.onFinishEditing.subscribe(
      calendar =>  { 
        if (calendar) {
          Object.assign(this.calendar, calendar);
          this.calendarIsOnEditing = false;
        }
    });
  }

  getCalendar(): void {
    const code = this.route.snapshot.paramMap.get('code');
    if (code) {     
      this.apiService.getCalendar(code).subscribe(
        calendar => {
          this.cookieService.updateCalendarsCookie(calendar);
          this.calendar = calendar;        
      }); 
    } else {
      this.calendar = new Calendar();
      this.editCalendar();
    }
  }

  editCalendar(): void {    
    this.calendarService.startEditing(this.calendar);
    this.calendarIsOnEditing = true;
  }

  deleteCalendar(): void {
    this.apiService.deleteCalendar(this.calendar.code).subscribe(
      () => {
        this.cookieService.deleteCalendarFromCookie(this.calendar.code);
        this.router.navigate(["/"]);    
    });
  } 

  createNote(): void {
    this.calendarService.selectNote(this.calendar.code, new Note());
  }
  
}
