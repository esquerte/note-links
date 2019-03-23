import { Component, OnInit, Input } from '@angular/core';

import {
  CalendarEvent, CalendarView, DAYS_OF_WEEK, CalendarDateFormatter, MOMENT
} from 'angular-calendar';

import { Observable, Subject } from 'rxjs';
import { map, takeUntil, find, filter } from 'rxjs/operators';
import * as moment from 'moment';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { Filter } from '../models/filter';
import { InteractionService } from '../services/interaction.service';
import { SignalRService } from '../services/signal-r.service';
import { CustomDateFormatter } from './custom-date-formatter';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';

// weekStartsOn option is ignored when using moment, as it needs to be configured globally for the moment locale
moment.updateLocale('en', {
  week: {
    dow: DAYS_OF_WEEK.MONDAY,
    doy: 0
  }
});

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: [
    // '../../../node_modules/angular-calendar/css/angular-calendar.css',
    './date-picker.component.scss'
  ],
  providers: [
    { provide: MOMENT, useValue: moment },
    { provide: CalendarDateFormatter, useClass: CustomDateFormatter }
  ]
})
export class DatePickerComponent implements OnInit {

  @Input() calendarCode: string;

  private selectedNoteId: number;

  private unsubscribe: Subject<void> = new Subject();

  view: CalendarView = CalendarView.Month;
  viewDate: moment.Moment = moment();

  years: Array<number> = new Array<number>();
  selectedMonth: number = moment().month() + 1;
  selectedYear: number = moment().year();

  events$: Observable<CalendarEvent<{ note: Note }>[]>;
  locale: string;

  activeDayIsOpen: boolean = false;

  constructor(
    private apiService: ApiService,
    private interactionService: InteractionService,
    private signalRService: SignalRService,
    private translate: TranslateService,
  ) {
    for (var i = 1900; i <= 2100; i++)
      this.years.push(i);
    this.locale = this.translate.currentLang;
  }

  ngOnInit(): void {

    this.fetchEvents();

    this.interactionService.onNoteFinishEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onNoteFinishEditing()
    );
    this.interactionService.onNoteSelected$.pipe(takeUntil(this.unsubscribe)).subscribe(
      ([calendarCode, note]) => this.onNoteSelected(note)
    );
    this.interactionService.onNoteDeleted$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onNoteDeleted(note)
    );
    this.translate.onLangChange.pipe(takeUntil(this.unsubscribe)).subscribe(
      (event: LangChangeEvent) => this.locale = event.lang
    )
    this.signalRService.onUpdate$.pipe(takeUntil(this.unsubscribe)).subscribe(
      calendaCode => {
        if (this.calendarCode == calendaCode) {
          this.fetchEvents();
        }
      });
  }

  fetchEvents(): void {

    let monthStart: string;
    let monthEnd: string;

    monthStart = moment(this.viewDate).startOf('month').format("YYYY-MM-DD HH:mm");
    monthEnd = moment(this.viewDate).endOf('month').format("YYYY-MM-DD HH:mm");

    let filters: Filter[] = [
      { field: "FromDate", operator: "ge", value: monthStart },
      { field: "FromDate", operator: "le", value: monthEnd }
    ];

    this.events$ = this.apiService.getCalendarNotes(this.calendarCode, filters, null)
      .pipe(
        map(({ notes }: { notes: Note[] }) => {
          return notes.map((note: Note) => {
            return {
              title: note.name,
              start: new Date(note.fromDate),
              end: new Date(note.toDate),
              allDay: true,
              meta: { note }
            }
          })
        })
      );
  }

  private onNoteFinishEditing() {
    this.fetchEvents();
  }

  private onNoteSelected(note: Note) {
    if (!moment(this.viewDate).isSame(note.fromDate, 'month')) {
      this.viewDate = moment(note.fromDate);
      this.changeSelectedMonth()
      this.fetchEvents();
    }
  }

  private onNoteDeleted(note: Note) {
    if (moment(this.viewDate).isSame(note.fromDate, 'month')) {
      this.fetchEvents();
    }
  }

  private changeSelectedMonth() {
    this.selectedMonth = moment(this.viewDate).month() + 1;
    this.selectedYear = moment(this.viewDate).year();
  }

  monthSelected() {
    this.viewDate = moment().year(this.selectedYear).month(this.selectedMonth - 1);
    this.fetchEvents();
  }

  viewDateChanged() {
    this.activeDayIsOpen = false;
    this.changeSelectedMonth()
    this.fetchEvents();
  }

  dayClicked({
    date,
    events
  }: {
    date: moment.Moment;
    events: CalendarEvent<{ note: Note }>[];
  }): void {

    this.interactionService.selectDate(date);

    if (
      (moment(this.viewDate).isSame(date) && this.activeDayIsOpen === true) ||
      events.length === 0
    ) {
      this.activeDayIsOpen = false;
    } else {
      this.activeDayIsOpen = true;
      this.viewDate = date;
    }

  }

  eventClicked(event: CalendarEvent<{ note: Note }>): void {
    this.interactionService.selectNote(this.calendarCode, event.meta.note);
    this.selectedNoteId = event.meta.note.id;
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
