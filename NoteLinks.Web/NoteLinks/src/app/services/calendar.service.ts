import { Injectable } from '@angular/core';
import { Subject, BehaviorSubject } from 'rxjs';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {

  constructor() { }

  // Calendar

  private startEditingSubject = new BehaviorSubject<Calendar>(null);
  onStartEditing$ = this.startEditingSubject.asObservable();

  private finishEditingSubject = new Subject<Calendar>();
  onFinishEditing$ = this.finishEditingSubject.asObservable();

  startEditing(calendar: Calendar): void {
    this.startEditingSubject.next(calendar);
  }

  finishEditing(calendar: Calendar): void {
    this.finishEditingSubject.next(calendar);
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

}