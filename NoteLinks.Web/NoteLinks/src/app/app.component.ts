import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router"
import 'hammerjs';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

import { Calendar } from './models/calendar';
import { CalendarCookieService } from './services/calendar-cookie.service'
import { InteractionService } from './services/interaction.service';
import { MatSnackBar } from '@angular/material';
import { UserService } from './services/user.service';
import { UserCalendar } from './models/user-calendar';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  title = 'NoteLinks';

  calendar: Calendar;

  cookieCalendars: Calendar[];
  followedCalendars: UserCalendar[];
  createdCalendars: UserCalendar[];

  get displayName(): string {
    return this.userService.getDisplayName();
  }

  public get loggedIn(): boolean {
    return this.userService.isLoggedIn();
  }

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cookieService: CalendarCookieService,
    public translate: TranslateService,
    private interactionService: InteractionService,
    private snackBar: MatSnackBar,
    private userService: UserService,
  ) {
    translate.addLangs(['en', 'ru']);
    translate.setDefaultLang('en');
    const browserLang = translate.getBrowserLang();
    translate.use(browserLang.match(/en|ru/) ? browserLang : 'en');
  }

  ngOnInit() {
    this.cookieService.onCalendarsChanged$.subscribe(
      () => this.cookieCalendars = this.getCookieCalendars()
    );
    this.interactionService.onSwitchCalendar$.pipe(takeUntil(this.unsubscribe)).subscribe(
      (calendar: Calendar) => {
        this.calendar = calendar;
        if (this.userService.isLoggedIn()) {
          this.getUserCalendars();
        }
      }
    );
    this.interactionService.onErrorOccured$.pipe(takeUntil(this.unsubscribe)).subscribe(
      error => this.showErrorMessage(error)
    );
    this.userService.onUserLoggedIn$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.onUserLoggedIn()
    );
    this.userService.onUserLoggedOut$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.onUserLoggedOut()
    );
    this.userService.onCalendarFollowed$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.getUserCalendars()
    );
    this.userService.onCalendarUnfollowed$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.getUserCalendars()
    );

    if (this.userService.isLoggedIn()) {
      this.onUserLoggedIn();
    } else {
      this.cookieCalendars = this.getCookieCalendars();
    }
    
  }

  private getCookieCalendars(): Calendar[] {

    let cookieCalendars: Calendar[] = this.cookieService.getCalendarsFromCookie();

    if (this.followedCalendars) {
      cookieCalendars = cookieCalendars.filter(x => !this.followedCalendars.find(y => y.code == x.code));
    }
    if (this.createdCalendars) {
      cookieCalendars = cookieCalendars.filter(x => !this.createdCalendars.find(y => y.code == x.code));
    }
    
    return cookieCalendars;    
  }

  selectCalendar(code: string) {
    this.router.navigate(['/calendars', code])
  }

  createCalendar() {
    this.router.navigate(['/calendars'])
  }

  editCalendar() {
    this.interactionService.editCalendar();
  }

  deleteCalendar() {
    this.interactionService.deleteCalendar();
  }

  logout() {
    this.userService.logout();
  }

  followCalendar() {
    this.userService.followCalendar();
  }

  unfollowCalendar() {
    this.userService.unfollowCalendar();
  }

  canBeFollowed(): boolean {
    if (this.cookieCalendars && this.cookieCalendars.find(x => x.code == this.calendar.code)) {
      return true;
    }
  }

  canBeUnfollowed(): boolean {
    if (this.followedCalendars && this.followedCalendars.find(x => x.code == this.calendar.code)) {
      return true;
    }
  }

  onUserLoggedIn() {
    this.getUserCalendars();
  }

  onUserLoggedOut() {
    this.followedCalendars = null;
    this.createdCalendars = null;
    this.cookieCalendars = this.cookieService.getCalendarsFromCookie();
  }

  private getUserCalendars() {
    this.userService.getCalendars().subscribe(
      (calendars: UserCalendar[]) => {
        this.followedCalendars = calendars.filter(x => !x.creator);
        this.createdCalendars = calendars.filter(x => x.creator);
        this.cookieCalendars = this.getCookieCalendars();
      },
      error => {
        this.interactionService.handleError(error);
      }
    );
  }

  showErrorMessage(error: any) {
    this.snackBar.open(error, "", {
      duration: 5000,
      panelClass: ['error-snack-bar']
    });
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
