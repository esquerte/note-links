import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {

  constructor() { }

  // Calendar

  private startEditingSubject = new BehaviorSubject<Calendar>(undefined);
  onStartEditing = this.startEditingSubject.asObservable();

  private finishEditingSubject = new BehaviorSubject<Calendar>(undefined);
  onFinishEditing = this.finishEditingSubject.asObservable();

  startEditing(calendar: Calendar): void {
    this.startEditingSubject.next(calendar);
  }

  finishEditing(calendar: Calendar): void {
    this.finishEditingSubject.next(calendar);
  }

  // Note

  private noteSelectedSubject = new BehaviorSubject<[string, Note]>(["", undefined]);
  onNoteSelected = this.noteSelectedSubject.asObservable();

  private noteStartEditingSubject = new BehaviorSubject<[string, Note]>(["", undefined]);
  onNoteStartEditing = this.noteStartEditingSubject.asObservable();

  private noteFinishEditingSubject = new BehaviorSubject<Note>(undefined);
  onNoteFinishEditing = this.noteFinishEditingSubject.asObservable();

  private noteDeletedSubject = new BehaviorSubject<Note>(undefined);
  onNoteDeleted = this.noteDeletedSubject.asObservable();

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
