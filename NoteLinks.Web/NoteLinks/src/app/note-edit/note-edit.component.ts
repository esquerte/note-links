import { Component, OnInit, OnDestroy, Directive, Input } from '@angular/core';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import * as moment from 'moment';
import { DateAdapter } from '@angular/material/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { InteractionService } from '../services/interaction.service'
import { SignalRService } from '../services/signal-r.service';
import { NgxMaterialTimepickerTheme } from 'ngx-material-timepicker';
import { DateFormatService } from '../services/date-format.service';

interface TimeRange {
  fromDate: moment.Moment;
  toDate: moment.Moment;
  fromTime: string;
  toTime: string;
}

@Component({
  selector: 'app-note-edit',
  templateUrl: './note-edit.component.html',
  styleUrls: ['./note-edit.component.scss']
})
export class NoteEditComponent implements OnInit, OnDestroy {

  note: Note;
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

  blueTheme: NgxMaterialTimepickerTheme = {
    container: {
      bodyBackgroundColor: '#ffffff',
      buttonColor: '#81d4fa'
    },
    dial: {
      dialBackgroundColor: '#81d4fa',
    },
    clockFace: {
      clockHandColor: '#81d4fa',
    }
  };

  constructor(
    private apiService: ApiService,
    private interactionService: InteractionService,
    private translate: TranslateService,
    private dateAdapter: DateAdapter<any>,
    private signalRService: SignalRService,
  ) { }

  ngOnInit() {
    this.interactionService.onNoteStartEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      ([calendarCode, note]) => {
        this.setLocalization();
        this.onStartEditing([calendarCode, note]);
      });
    this.interactionService.onCalendarDateSelected$.pipe(takeUntil(this.unsubscribe)).subscribe(
      date => this.onCalendarDateChanged(date)
    );
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
    this.interactionService.noteCancelEditing(this.note);
  }

  private updateNote() {
    this.makeDates();
    this.apiService.updateNote(this.note).subscribe(
      note => {
        this.interactionService.noteFinishEditing(note);
        this.signalRService.change(this.calendarCode);
      },
      error => this.interactionService.handleError(error)
    );
  }

  private createNote() {
    this.makeDates();
    this.apiService.createNote(this.calendarCode, this.note).subscribe(
      note => {
        this.interactionService.noteFinishEditing(note);
        this.signalRService.change(this.calendarCode);
      },
      error => this.interactionService.handleError(error)
    );
  }

  private onStartEditing([calendarCode, note]: [string, Note]) {
    this.note = note;
    this.calendarCode = calendarCode;
    if (note.id) {
      this.setTimeRange();
      this.toTimeMinValue = this.getToTimeMinValue();
    } else {
      this.timeRange = {} as TimeRange;
    }
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
    if (this.timeRange.fromDate && moment(this.timeRange.fromDate).isSame(this.timeRange.toDate)) {
      return moment(this.timeRange.fromTime, this.timeFormat);
    } else {
      return moment().hour(0).minute(0);
    }
  }

  private setTimeRange() {
    let fromDate = moment(this.note.fromDate);
    this.timeRange.fromDate = moment([fromDate.year(), fromDate.month(), fromDate.date()]);
    this.timeRange.fromTime = fromDate.format(this.timeFormat);
    if (this.note.toDate) {
      let toDate = moment(this.note.toDate);
      this.timeRange.toDate = moment([toDate.year(), toDate.month(), toDate.date()]);
      this.timeRange.toTime = toDate.format(this.timeFormat);
    }
  }

  private onCalendarDateChanged(date: moment.Moment) {
    if (this.timeRange.fromDate && moment(date).isSameOrAfter(this.timeRange.fromDate)) {
      this.timeRange.toDate = moment(date);
    } else {
      this.timeRange.fromDate = moment(date);
      this.timeRange.toDate = null;
    }
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

    if (this.timeRange.toTime) {

      // if minValue is greater than value of toTime, toTime doesn't change format
      this.toTimeMinValue = moment().hour(0).minute(0);

      this.timeRange.toTime = moment(this.timeRange.toTime, oldFormat).format(this.timeFormat);
    }

  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
