import { Component, OnInit, Input } from '@angular/core';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { CalendarService } from '../services/calendar.service'

@Component({
  selector: 'app-note-edit',
  templateUrl: './note-edit.component.html',
  styleUrls: ['./note-edit.component.css']
})
export class NoteEditComponent implements OnInit {

  note: Note;
  originalNote: Note;
  calendarCode: string;

  constructor(
    private apiService: ApiService,
    private calendarService: CalendarService,
  ) {}

  ngOnInit() {
    this.calendarService.onNoteStartEditing.subscribe(
      ([calendarCode, note]) =>  { 
        if (note) {          
          this.note = note;
          this.originalNote = Object.assign({}, note);
          this.calendarCode = calendarCode;
        }
    });
  }

  saveNote(): void {
    if (this.note.id) {
      this.updateNote();
    } else {
      this.createNote();
    } 
  }

  private updateNote(): void {
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

  cancelEditing(): void {
    this.calendarService.noteFinishEditing(this.originalNote);
  }

}
