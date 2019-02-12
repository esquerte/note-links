import { Injectable } from '@angular/core';
import { Subject, BehaviorSubject } from 'rxjs';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {

  constructor() { }

  // Calendar

  private editCalendarSubject = new Subject();
  onEditCalendar$ = this.editCalendarSubject.asObservable();

  private startEditingSubject = new BehaviorSubject<Calendar>(null);
  onStartEditing$ = this.startEditingSubject.asObservable();

  private finishEditingSubject = new Subject<Calendar>();
  onFinishEditing$ = this.finishEditingSubject.asObservable();

  private deleteCalendarSubject = new Subject();
  onDeleteCalendar$ = this.deleteCalendarSubject.asObservable();

  editCalendar(): void {
    this.editCalendarSubject.next();
  }

  startEditing(calendar: Calendar): void {
    this.startEditingSubject.next(calendar);
  }

  finishEditing(calendar: Calendar): void {
    this.finishEditingSubject.next(calendar);
  }

  deleteCalendar(): void {
    this.deleteCalendarSubject.next();
  }

  // Note

  private noteSelectedSubject = new Subject<[string, Note]>();
  onNoteSelected$ = this.noteSelectedSubject.asObservable();

  private noteStartEditingSubject = new BehaviorSubject<[string, Note]>(["", null]);
  onNoteStartEditing$ = this.noteStartEditingSubject.asObservable();

  private noteFinishEditingSubject = new Subject<Note>();
  onNoteFinishEditing$ = this.noteFinishEditingSubject.asObservable();

  private noteDeletedSubject = new Subject<Note>();
  onNoteDeleted$ = this.noteDeletedSubject.asObservable();

  private fromDateSelectedSubject = new Subject<moment.Moment>();
  onFromDateSelected$ = this.fromDateSelectedSubject.asObservable();

  private toDateSelectedSubject = new Subject<moment.Moment>();
  onToDateSelected$ = this.toDateSelectedSubject.asObservable();

  selectNote(calendarCode: string, note: Note): void {
    this.noteSelectedSubject.next([calendarCode, note]);
  }

  noteStartEditing(calendarCode: string, note: Note): void {
    this.noteStartEditingSubject.next([calendarCode, note]);
  }

  noteFinishEditing(note: Note): void {
    this.noteFinishEditingSubject.next(note);
  }

  deleteNote(note: Note): void {
    this.noteDeletedSubject.next(note);
  }

  selectFromDate(fromDate: moment.Moment): void {
    this.fromDateSelectedSubject.next(fromDate);
  }

  selectToDate(toDate: moment.Moment): void {
    this.toDateSelectedSubject.next(toDate);
  }

}
