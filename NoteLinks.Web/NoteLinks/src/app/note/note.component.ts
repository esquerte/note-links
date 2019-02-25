import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { Note } from '../models/note';
import { CalendarService } from '../services/calendar.service'

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss']
})
export class NoteComponent implements OnInit, OnDestroy {

  note: Note;
  noteIsOnEditing: boolean;
  private calendarCode: string;

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private calendarService: CalendarService
  ) {}

  ngOnInit() {
    this.calendarService.onNoteSelected$.pipe(takeUntil(this.unsubscribe)).subscribe(
      ([calendarCode, note]) => this.onSelected([calendarCode, note])
    );
    this.calendarService.onNoteFinishEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onFinishEditing(note)
    );
    this.calendarService.onNoteDeleted$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onDeleted(note)
    );  
  }

  editNote(): void {
    this.calendarService.noteStartEditing(this.calendarCode, this.note);
    this.noteIsOnEditing = true;
  }

  private onSelected([calendarCode, note]): void {
    this.noteIsOnEditing = false; 
    this.note = note;
    this.calendarCode = calendarCode;
    if (!note.id) {
      this.editNote();
    }
  }

  private onFinishEditing(note: Note): void {
    if (note.id) {
      Object.assign(this.note, note);      
    } else {
      this.note = null;
    }
    this.noteIsOnEditing = false;
  }

  private onDeleted(note: Note): void {
    if (note.id == this.note.id) {
      this.note = null;
      this.noteIsOnEditing = false;
    }
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
