import { Component, OnInit, Input } from '@angular/core';

import { Calendar } from '../models/calendar';

@Component({
  selector: 'app-calendar-info',
  templateUrl: './calendar-info.component.html',
  styleUrls: ['./calendar-info.component.css']
})
export class CalendarInfoComponent implements OnInit {

  @Input() calendar: Calendar;

  constructor() { }

  ngOnInit() {
  }

}
