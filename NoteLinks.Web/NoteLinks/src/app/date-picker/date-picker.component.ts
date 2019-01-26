import { Component, OnInit, Input } from '@angular/core';
import { CalendarEvent, CalendarView, DAYS_OF_WEEK, CalendarMonthViewDay } from 'angular-calendar';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as moment from 'moment';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { PageInfo } from '../models/page-info';
import { Filter } from '../models/filter';

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

  constructor(
    private apiService: ApiService,
  ) {}

  ngOnInit(): void {
    this.fetchEvents();
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

    let pageInfo: PageInfo = {
      pageIndex: 1,
      pageSize: 5,   
      totalCount: 0,
      orderBy: "",
      desc: false   
    }
    
    // http://localhost/api/Notes/asdf?filters[0].Field=Name&filters[0].Operator=eq&filters[0].Value=%D0%9C%D0%B0%D1%80%D0%B8%D0%BD%D0%B0&pageSize=5&pageIndex=1
    this.events$ = this.apiService.getCalendarNotes(this.calendarCode, filters, null)
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
