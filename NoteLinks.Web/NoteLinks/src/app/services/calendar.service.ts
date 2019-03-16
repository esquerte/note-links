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

  private noteIsOnEditing: boolean;

  // Calendar

  private editCalendarSubject = new Subject();
  onEditCalendar$ = this.editCalendarSubject.asObservable();

  private startEditingSubject = new BehaviorSubject<Calendar>(null);
  onStartEditing$ = this.startEditingSubject.asObservable();

  private finishEditingSubject = new Subject<Calendar>();
  onFinishEditing$ = this.finishEditingSubject.asObservable();

  private deleteCalendarSubject = new Subject();
  onDeleteCalendar$ = this.deleteCalendarSubject.asObservable();

  private errorOccuredSubject = new Subject<string>();
  onErrorOccured$ = this.errorOccuredSubject.asObservable();

  private calendarDateSelectedSubject = new Subject<moment.Moment>();
  onCalendarDateSelected$ = this.calendarDateSelectedSubject.asObservable();

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

  handleError(message: string): void {
    this.errorOccuredSubject.next(message);
  }

  selectDate(date: moment.Moment): void {
    this.calendarDateSelectedSubject.next(date);
  }

  // Note

  private noteSelectedSubject = new Subject<[string, Note]>();
  onNoteSelected$ = this.noteSelectedSubject.asObservable();

  private noteStartEditingSubject = new BehaviorSubject<[string, Note]>(["", null]);
  onNoteStartEditing$ = this.noteStartEditingSubject.asObservable();

  private noteFinishEditingSubject = new Subject<Note>();
  onNoteFinishEditing$ = this.noteFinishEditingSubject.asObservable();

  private noteCancelEditingSubject = new Subject<Note>();
  onNoteCancelEditing$ = this.noteCancelEditingSubject.asObservable();

  private noteDeletedSubject = new Subject<Note>();
  onNoteDeleted$ = this.noteDeletedSubject.asObservable();

  selectNote(calendarCode: string, note: Note): void {
    this.noteSelectedSubject.next([calendarCode, note]);
  }

  noteStartEditing(calendarCode: string, note: Note): void {
    this.noteIsOnEditing = true;
    this.noteStartEditingSubject.next([calendarCode, note]);    
  }

  noteFinishEditing(note: Note): void {
    this.noteIsOnEditing = false;
    this.noteFinishEditingSubject.next(note);    
  }
  
  noteCancelEditing(note: Note): void {
    this.noteIsOnEditing = false;
    this.noteCancelEditingSubject.next(note);    
  }

  deleteNote(note: Note): void {
    this.noteDeletedSubject.next(note);
  }

  isNoteOnEditing(): boolean {
    return this.noteIsOnEditing;
  };

}
