import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import * as moment from 'moment';
import {DateAdapter } from '@angular/material/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

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
export class NoteEditComponent implements OnInit, OnDestroy {

  note: Note;
  private originalNote: Note;
  private calendarCode: string;

  timeRange: TimeRange = {} as TimeRange;
  toTimeMinValue: moment.Moment;
  timePickerFormat: number;

  private unsubscribe: Subject<void> = new Subject();

  private get timeFormat(): string {
    switch (this.timePickerFormat) {
      case 12: return "hh:mm a"
      case 24: return "HH:mm"
      default: return "hh:mm a"
    }
  }

  constructor(
    private apiService: ApiService,
    private calendarService: CalendarService,
    private translate: TranslateService,
    private dateAdapter: DateAdapter<any>,
  ) {}

  ngOnInit() {
    this.calendarService.onNoteStartEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(      
      ([calendarCode, note]) => {
        this.setLocalization();
        this.onStartEditing([calendarCode, note])
    });
    this.translate.onLangChange.pipe(takeUntil(this.unsubscribe)).subscribe(
      (event: LangChangeEvent) => {
        this.changeLocalization();
    });
  }

  saveNote() {
    if (this.note.id) {
      this.updateNote();
    } else {
      this.createNote();
    } 
  }

  cancelEditing() {    
    this.calendarService.noteFinishEditing(this.originalNote);
  }

  private updateNote() {
    this.makeDates();
    this.apiService.updateNote(this.note).subscribe(
      note => {
        this.calendarService.noteFinishEditing(note);
    });     
  }

  private createNote() {
    this.makeDates();
    this.apiService.createNote(this.calendarCode, this.note).subscribe(
      note => {           
        this.calendarService.noteFinishEditing(note);   
    });  
  }

  private onStartEditing([calendarCode, note]: [string, Note]) {        
    this.note = note;
    this.originalNote = Object.assign({}, note);
    this.calendarCode = calendarCode;
    if (note.id) 
      this.setTimeRange();   
  }

  private makeDates() {
    if (this.timeRange.fromTime) {
      let fromTime = moment(this.timeRange.fromTime, this.timeFormat);
      this.timeRange.fromDate.hour(fromTime.hour()).minute(fromTime.minute());
    }
    this.note.fromDate = this.timeRange.fromDate.format();
    if (this.timeRange.toDate) {
      if (this.timeRange.toTime) {
        let toTime = moment(this.timeRange.toTime, this.timeFormat);    
        this.timeRange.toDate.hour(toTime.hour()).minute(toTime.minute()); 
      }   
      this.note.toDate = this.timeRange.toDate.format();
    }
  }

  onDateChanged(event: MatDatepickerInputEvent<Date>) {
    this.toTimeMinValue = this.getToTimeMinValue();
  }

  onFromTimeChanged(event) {
    this.toTimeMinValue = this.getToTimeMinValue();
  }

  private getToTimeMinValue(): moment.Moment {
    if (moment(this.timeRange.fromDate).isSame(this.timeRange.toDate)) {
      return moment(this.timeRange.fromTime, this.timeFormat);
    } else {
      return moment().hour(0).minute(0);
    }
  }

  private setTimeRange() {
    let fromDate = moment(this.note.fromDate);
    let toDate = moment(this.note.toDate);
    this.timeRange.fromDate = moment([fromDate.year(), fromDate.month(), fromDate.date()]);
    this.timeRange.toDate = moment([toDate.year(), toDate.month(), toDate.date()]);
    this.timeRange.fromTime = fromDate.format(this.timeFormat);
    this.timeRange.toTime = toDate.format(this.timeFormat);
  }

  private setLocalization() {
    this.dateAdapter.setLocale(this.translate.currentLang);
    this.timePickerFormat = this.translate.currentLang == "en" ? 12 : 24;        
  }

  private changeLocalization() {
    
    this.dateAdapter.setLocale(this.translate.currentLang);

    let oldFormat: string = this.timeFormat
    this.timePickerFormat = this.translate.currentLang == "en" ? 12 : 24;    

    if (this.timeRange.fromTime)
      this.timeRange.fromTime = moment(this.timeRange.fromTime, oldFormat).format(this.timeFormat);

    if (this.timeRange.toTime)
      this.timeRange.toTime = moment(this.timeRange.toTime, oldFormat).format(this.timeFormat);
    
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}