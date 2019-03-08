import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router"
import 'hammerjs';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

import { Calendar } from './models/calendar';
import { CalendarCookieService } from './services/calendar-cookie.service'
import { CalendarService } from './services/calendar.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  title = 'NoteLinks';
  calendars: Calendar[];

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cookieService: CalendarCookieService,
    public translate: TranslateService,
    private calendarService: CalendarService,
    private snackBar: MatSnackBar,
  ) {
    cookieService.calendars$.subscribe(
      calendars => this.calendars = calendars
    );
    translate.addLangs(['en', 'ru']);
    translate.setDefaultLang('en');
    const browserLang = translate.getBrowserLang();
    translate.use(browserLang.match(/en|ru/) ? browserLang : 'en');
  }

  ngOnInit() {
    this.calendarService.onErrorOccured$.pipe(takeUntil(this.unsubscribe)).subscribe(
      error => this.showErrorMessage(error)
    );
  }

  selectCalendar(code: string) {
    this.router.navigate(['/calendars', code])
  }

  createCalendar() {
    this.router.navigate(['/calendars'])
  }

  editCalendar() {
    this.calendarService.editCalendar();
  }

  deleteCalendar() {
    this.calendarService.deleteCalendar();
  }

  showErrorMessage(error: any) {
    this.snackBar.open(error, "", {
      duration: 3000,
      panelClass: ['error-snack-bar']
    });
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
