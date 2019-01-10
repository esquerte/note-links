import { Component, OnInit } from '@angular/core';
import { CalendarEvent, CalendarMonthViewDay } from 'angular-calendar';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements OnInit {

  viewDate: Date = new Date();
  events: CalendarEvent[] = [];
  clickedDate: Date;

  constructor() { }

  ngOnInit() {
  }

}
