import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { Note } from '../models/note';
import { CalendarService } from '../services/calendar.service'
import { SignalRService } from '../services/signal-r.service';
import { Filter } from '../models/filter';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss']
})
export class NoteComponent implements OnInit, OnDestroy {

  note: Note;
  private calendarCode: string;

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    public calendarService: CalendarService,
    private signalRService: SignalRService,
    private apiService: ApiService,
  ) { }

  ngOnInit() {
    this.calendarService.onNoteSelected$.pipe(takeUntil(this.unsubscribe)).subscribe(
      ([calendarCode, note]) => this.onSelected([calendarCode, note])
    );
    this.calendarService.onNoteFinishEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onFinishEditing(note)
    );
    this.calendarService.onNoteCancelEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onCancelEditing(note)
    );
    this.calendarService.onNoteDeleted$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onDeleted(note)
    );
    this.signalRService.onUpdate$.pipe(takeUntil(this.unsubscribe)).subscribe(
      calendaCode => {
        if (this.calendarCode == calendaCode) {          
          this.updateNote();
        }
      });
  }

  editNote(): void {
    this.calendarService.noteStartEditing(this.calendarCode, this.note);
  }

  private onSelected([calendarCode, note]): void {
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
  }

  private onCancelEditing(note: Note) {
    this.updateNote();
  }

  private updateNote() {

    if (this.note && this.note.id && !this.calendarService.isNoteOnEditing()) {

      let filters: Filter[] = [
        { field: "Id", operator: "eq", value: this.note.id.toString() }
      ];

      this.apiService.getCalendarNotes(this.calendarCode, filters, null).subscribe(
        result => this.note = result.notes[0]
      );

    }

  }

  private onDeleted(note: Note): void {
    if (note.id == this.note.id) {
      this.note = null;
    }
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
