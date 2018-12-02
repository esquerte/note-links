import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router"
import 'hammerjs';
import  {TranslateService } from '@ngx-translate/core';

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
    private cookieService: CalendarCookieService,
    public translate: TranslateService,
  ) { 
    cookieService.calendars.subscribe(
      calendars => this.calendars = calendars
    );
    translate.addLangs(['en', 'ru']);
    translate.setDefaultLang('en');
    const browserLang = translate.getBrowserLang();
    translate.use(browserLang.match(/en|ru/) ? browserLang : 'en');
  }

  ngOnInit() {}

  selectCalendar(code: string): void {
    this.router.navigate(['/calendars', code])
  }

  createCalendar(): void {
    this.router.navigate(['/calendars'])
  }

}