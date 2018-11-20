import { Component, OnInit } from '@angular/core';

import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import * as moment from 'moment';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { CalendarService } from '../services/calendar.service'

interface TimeRange {
  fromDate: moment.Moment;
  toDate: moment.Moment;
  fromTime: string;
  toTime: string;
}

@Component({
  selector: 'app-note-edit',
  templateUrl: './note-edit.component.html',
  styleUrls: ['./note-edit.component.css']
})
export class NoteEditComponent implements OnInit {

  note: Note;
  originalNote: Note;
  calendarCode: string;

  timeRange: TimeRange = {} as TimeRange;
  toTimeMinValue: moment.Moment;

  constructor(
    private apiService: ApiService,
    private calendarService: CalendarService,
  ) {}

  ngOnInit() {
    this.calendarService.onNoteStartEditing$.subscribe(
      ([calendarCode, note]) => this.onStartEditing([calendarCode, note])
    );
  }

  saveNote(): void {
    if (this.note.id) {
      this.updateNote();
    } else {
      this.createNote();
    } 
  }

  cancelEditing(): void {
    this.calendarService.noteFinishEditing(this.originalNote);
  }

  private updateNote(): void {
    this.makeDates();
    this.apiService.updateNote(this.note).subscribe(
      note => {
        this.calendarService.noteFinishEditing(note);
    });     
  }

  private createNote(): void {
    this.apiService.createNote(this.calendarCode, this.note).subscribe(
      note => {           
        this.calendarService.noteFinishEditing(note);   
    });  
  }

  private onStartEditing([calendarCode, note]: [string, Note]): void {
    if (note) {          
      this.note = note;
      this.originalNote = Object.assign({}, note);
      this.calendarCode = calendarCode;
      this.setTimeRange();
    }    
  }

  private makeDates(): void {
    let fromTime = moment(this.timeRange.fromTime, "HH:mm");
    let toTime = moment(this.timeRange.toTime, "HH:mm");
    this.timeRange.fromDate.hour(fromTime.hour()).minute(fromTime.minute());
    this.timeRange.toDate.hour(toTime.hour()).minute(toTime.minute());
    this.note.fromDate = this.timeRange.fromDate.format();
    this.note.toDate = this.timeRange.toDate.format();
  }

  dateChanged(event: MatDatepickerInputEvent<Date>): void {
    this.toTimeMinValue = this.getToTimeMinValue();
  }

  fromTimeChanged(event): void {
    this.toTimeMinValue = this.getToTimeMinValue();
  }

  private getToTimeMinValue(): moment.Moment {
    if (moment(this.timeRange.fromDate).isSame(this.timeRange.toDate)) {
      return moment(this.timeRange.fromTime, "HH:mm");
    } else {
      return moment().hour(0).minute(0);
    }
  }

  private setTimeRange(): void {
    let fromDate = moment(this.note.fromDate).local();
    let toDate = moment(this.note.toDate).local();
    this.timeRange.fromDate = moment([fromDate.year(), fromDate.month(), fromDate.date()]);
    this.timeRange.toDate = moment([toDate.year(), toDate.month(), toDate.date()]);
    this.timeRange.fromTime = fromDate.format("HH:mm");
    this.timeRange.toTime = toDate.format("HH:mm");
  }

}
