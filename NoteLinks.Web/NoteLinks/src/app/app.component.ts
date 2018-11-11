import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router"
import 'hammerjs';

import { Calendar } from './models/calendar';
import { CalendarCookieService } from './services/calendar-cookie.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'NoteLinks';
  calendars: Calendar[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cookieService: CalendarCookieService
  ) { 
    cookieService.calendars.subscribe(calendars => this.calendars = calendars);
  }

  ngOnInit() {}

  selectCalendar(code: string): void {
    this.router.navigate(['/calendars', code])
  }

  addCalendar(): void {
    this.router.navigate(['/edit'])
  }

}
