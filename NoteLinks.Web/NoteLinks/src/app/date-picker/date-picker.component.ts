import { Component, OnInit, Input } from '@angular/core';
import { CalendarEvent, CalendarView, DAYS_OF_WEEK, CalendarMonthViewDay } from 'angular-calendar';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as moment from 'moment';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { PageInfo } from '../models/page-info';

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
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements OnInit {

  @Input() calendarCode: string;

  colors: any = {
    red: {
      primary: '#ad2121',
      secondary: '#FAE3E3'
    },
    blue: {
      primary: '#1e90ff',
      secondary: '#D1E8FF'
    },
    yellow: {
      primary: '#e3bc08',
      secondary: '#FDF1BA'
    }
  }

  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  events: CalendarEvent[] = [];
  clickedDate: Date;

  events$: Observable<Array<CalendarEvent<{ note: Note }>>>;

  activeDayIsOpen: boolean = false;

  pageInfo: PageInfo = {
    pageIndex: 1,
    pageSize: 20,
    totalCount: 0,
    orderBy: "Name",
    desc: false
  }

  constructor(
    private apiService: ApiService,
  ) {}

  ngOnInit(): void {
    this.fetchEvents();
  }

  fetchEvents(): void {

    let fromDate: string;
    let toDate: string;

    fromDate = moment(this.viewDate).startOf('month').format();
    toDate = moment(this.viewDate).endOf('month').format();
    
    this.events$ = this.apiService.getCalendarNotes(this.calendarCode, this.pageInfo)
    .pipe(
      map(({ notes }: { notes: Note[] }) => {
        return notes.map((note: Note) => {
          return {
            title: note.name,
            start: new Date(note.fromDate),
            end: new Date(note.toDate),
            color: this.colors.red,
            allDay: true,
            meta: { note }
          };
        });
      })        
    );
  }

  dayClicked({
    date,
    events
  }: {
    date: Date;
    events: Array<CalendarEvent<{ note: Note }>>;
  }): void {
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
   
  }

}
