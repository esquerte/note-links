import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

import { Calendar } from '../models/calendar';
import { CalendarService } from '../services/calendar.service';

@Component({
  selector: 'app-calendar-edit',
  templateUrl: './calendar-edit.component.html',
  styleUrls: ['./calendar-edit.component.css']
})
export class CalendarEditComponent implements OnInit {

  calendar: Calendar;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private calendarService: CalendarService,
    private location: Location
  ) {
      route.params.subscribe(() => {
        this.editCalendar()
      });
  }

  ngOnInit() {}

  editCalendar(): void {
    const code = this.route.snapshot.paramMap.get('code');   
    if (code) { 
      this.calendarService.getCalendar(code).subscribe(calendar => {
        this.calendar = calendar;
      });
    } else {
      this.calendar = new Calendar();
    }
  }

  saveCalendar(): void {
    if (this.calendar.code != null) {
      this.updateCalendar();
    } else {
      this.createCalendar();
    } 
  }

  private updateCalendar(): void {
    this.calendarService.updateCalendar(this.calendar).subscribe(
      () => this.goBack()
    );     
  }

  private createCalendar(): void {
    this.calendarService.createCalendar(this.calendar).subscribe(
      calendar => {           
        this.router.navigate(['/calendars', calendar.code]);      
      }
    );  
  }

  goBack(): void {
    this.location.back();
  }

}
