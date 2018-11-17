import { Component, OnInit, Input } from '@angular/core';

import { Note } from '../models/note';
import { CalendarService } from '../services/calendar.service'
import { Calendar } from '../models/calendar';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.css']
})
export class NoteComponent implements OnInit {

  note: Note;
  noteIsOnEditing: boolean;
  calendarCode: string;

  constructor(
    private calendarService: CalendarService,
  ) { }

  ngOnInit() {
    this.calendarService.onNoteSelected.subscribe(
      ([calendarCode, note]) => this.onSelected([calendarCode, note])
    );
    this.calendarService.onNoteFinishEditing.subscribe(
      note => this.onFinishEditing(note)
    );
    this.calendarService.onNoteDeleted.subscribe(
      note => this.onDeleted(note)
    ); 
  }

  editNote(): void {
    this.calendarService.noteStartEditing(this.calendarCode, this.note);
    this.noteIsOnEditing = true;
  }

  private onSelected([calendarCode, note]): void {
    if (!this.noteIsOnEditing && note) {
      this.note = note;
      this.calendarCode = calendarCode;
      if (!note.id) {
        this.editNote();
      }
    }
  }

  private onFinishEditing(note: Note): void {
    if (note) {
      Object.assign(this.note, note);
      this.noteIsOnEditing = false;
    }
  }

  private onDeleted(note: Note): void {
    if (note && note.id == this.note.id) {
      this.note = null;
      this.noteIsOnEditing = false;
    }
  }

}
