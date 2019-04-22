import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service'
import { BehaviorSubject, Subject } from 'rxjs';

import { Calendar } from '../models/calendar';

@Injectable({
  providedIn: 'root'
})
export class CalendarCookieService {

  cookieName: string = "calendars";
  numberOfDays: number = 1825;
  path: string = "/";

  // private calendarsChangedSubject = new BehaviorSubject<Calendar[]>(this.getCalendarsFromCookie());
  private calendarsChangedSubject = new Subject();
  onCalendarsChanged$ = this.calendarsChangedSubject.asObservable();

  constructor(private cookieService: CookieService) { }

  getCalendarsFromCookie(): Calendar[] {
    let calendars: Calendar[] = [];
    let calendarsCookie: string = this.cookieService.get(this.cookieName)
    if (calendarsCookie) calendars = JSON.parse(calendarsCookie);
    return calendars;
  }

  deleteCalendarFromCookie(code: string): void {
    let calendars = this.getCalendarsFromCookie();
    calendars = calendars.filter(x => x.code != code); 
    if (calendars.length > 0) {
      this.cookieService.set(this.cookieName, JSON.stringify(calendars), this.numberOfDays, this.path);
    } else {
      this.cookieService.delete(this.cookieName, this.path);
    }
    this.calendarsChangedSubject.next();
  }

  updateCalendarsCookie(calendar: Calendar): void {
    let calendars = this.getCalendarsFromCookie();
    let cookieCalendar = calendars.find(x => x.code == calendar.code)
    if (!cookieCalendar || cookieCalendar.name != calendar.name) {
      calendars = calendars.filter(x => x.code != calendar.code);
      calendars.splice(0, 0, calendar);
      this.cookieService.set(this.cookieName, JSON.stringify(calendars), this.numberOfDays, this.path); 
      this.calendarsChangedSubject.next();     
    }
  }

}
