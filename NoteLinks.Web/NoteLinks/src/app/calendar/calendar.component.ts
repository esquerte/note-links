import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { Calendar } from '../calendar';
import { CalendarService } from '../calendar.service';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {

  calendar: Calendar;

  constructor(
    private route: ActivatedRoute,
    private calendarService: CalendarService,
    private location: Location
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
    this.calendarService.getCalendar(code).subscribe(calendar => this.calendar = calendar);
  }

}
