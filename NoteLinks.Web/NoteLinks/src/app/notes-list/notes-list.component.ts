import { Component, OnInit, Input } from '@angular/core';

import { Note } from '../models/note';
import { ApiService } from '../services/api.service';
import { CalendarService } from '../services/calendar.service';
import { MatTableDataSource } from '@angular/material'

@Component({
  selector: 'app-notes-list',
  templateUrl: './notes-list.component.html',
  styleUrls: ['./notes-list.component.css']
})
export class NotesListComponent implements OnInit {

  @Input() calendarCode: string;

  notes = new MatTableDataSource<Note>();
  selectedNoteId: number;

  displayedColumns: string[] = ['name', 'fromDate', 'toDate', 'text', 'action'];

  constructor(
    private apiService: ApiService,
    private calendarService: CalendarService,
  ) { }

  ngOnInit() {
    this.getNotes();
    this.calendarService.onNoteFinishEditing$.subscribe(
      note => this.onFinishEditing(note)
    );
  }

  getNotes(): void {
    this.apiService.getCalendarNotes(this.calendarCode).subscribe(
      notes => {
        this.notes.data = notes;
    });    
  }

  selectNote(note: Note): void {
    this.calendarService.selectNote(this.calendarCode, note);
    this.selectedNoteId = note.id;
  }

  deleteNote(note: Note): void {
    this.apiService.deleteNote(note.id).subscribe(
      () => {
        this.notes.data = this.notes.data.filter(x => x.id !== note.id);
        this.calendarService.deleteNote(note); 
      }
    );
  }

  private onFinishEditing(note: Note): void {
    if (note) {
      if (!this.notes.data.find(x => x.id == note.id)) {
        this.notes.data.push(note);
        // table updates only when overwrite the whole array
        this.notes.data = this.notes.data.slice();
      }
    }
  }

}
