import { Component, OnInit, Input } from '@angular/core';
import { Note } from '../note';
import { CalendarService } from '../calendar.service';

@Component({
  selector: 'app-notes-list',
  templateUrl: './notes-list.component.html',
  styleUrls: ['./notes-list.component.css']
})
export class NotesListComponent implements OnInit {

  @Input() calendarCode: string;

  notes: Note[];
  selectedNote: Note;
  editingNote: Note;
  originalNote: Note;

  constructor(private calendarService: CalendarService) { }

  ngOnInit() {
    this.getNotes();
  }

  getNotes(): void {
    this.calendarService.getCalendarNotes(this.calendarCode).subscribe(notes => this.notes = notes);
  }

  selectNote(note: Note): void {
    if (this.editingNote == null) {
      this.selectedNote = note;
    }
  }

  editNote(note: Note): void {
    this.originalNote = Object.assign({}, note);
    this.editingNote = note;
  }

  addNote(): void {
    this.selectedNote = null;
    this.editingNote = new Note();
  }

  saveNote(): void {
    if (this.editingNote.id != null) {
      this.updateNote();
    } else {
      this.createNote();
    } 
  }

  private updateNote(): void {
    this.calendarService.updateNote(this.editingNote).subscribe(
      () => this.editingNote = null, 
      () => this.cancelChanges()
    );  
  }

  private createNote(): void {
    this.calendarService.createNote(this.calendarCode, this.editingNote).subscribe(
      note => {
        this.notes.push(note);     
        this.editingNote = null; 
        this.selectedNote = note;      
      },
      () => this.editingNote = null
    );
  }

  deleteNote(note: Note): void {
    this.calendarService.deleteNote(note.id).subscribe(
      () => {
        this.notes = this.notes.filter(n => n !== note); 
        this.editingNote = null;  
        this.selectedNote = null;  
      }
    );
  }

  cancelChanges(): void {    
    this.editingNote = null;  
    Object.assign(this.selectedNote, this.originalNote);
  }

}
