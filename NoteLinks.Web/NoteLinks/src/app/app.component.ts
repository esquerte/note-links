import { Component, OnInit } from '@angular/core';
import {Router} from "@angular/router"

import { Calendar } from './calendar';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'NoteLinks';
  calendars: Calendar[] = [
    {name: "test1", code: "asdf"},
    {name: "test2", code: "qwer"},
    {name: "test3", code: "zxcv"}
  ];

  constructor(private router: Router) { }

  ngOnInit() {
    if (this.calendars.length > 0) {
      this.router.navigate(['/calendars', this.calendars[0].code]);
    }
  }

  selectCalendar(code: string): void {
    this.router.navigate(['/calendars', code])
  }

}
