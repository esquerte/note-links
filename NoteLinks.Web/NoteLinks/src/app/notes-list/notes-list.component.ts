import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { PageEvent, MatPaginatorIntl, MatDialog } from '@angular/material';
import { Sort } from '@angular/material';

import { Note } from '../models/note';
import { PageInfo } from '../models/page-info';
import { ApiService } from '../services/api.service';
import { CalendarService } from '../services/calendar.service';
import { MatTableDataSource } from '@angular/material'
import { SignalRService } from '../services/signal-r.service';
import { CustomPaginatorIntl } from './custom-paginator-intl';
import { DeleteNoteDialogComponent } from '../delete-note-dialog/delete-note-dialog.component';

@Component({
  selector: 'app-notes-list',
  templateUrl: './notes-list.component.html',
  styleUrls: ['./notes-list.component.scss'],
  providers: [
    { provide: MatPaginatorIntl, useClass: CustomPaginatorIntl }
  ]
})
export class NotesListComponent implements OnInit, OnDestroy {

  @Input() calendarCode: string;

  notes = new MatTableDataSource<Note>();
  selectedNoteId: number;

  private unsubscribe: Subject<void> = new Subject();

  displayedColumns: string[] = ['name', 'fromDate', 'toDate', 'text', 'action'];

  pageSizeOptions: number[] = [2, 5, 10, 25];
  isLoading = true;

  pageInfo: PageInfo = {
    pageIndex: 1,
    pageSize: 2,
    totalCount: 0,
    orderBy: "Name",
    desc: false
  }

  constructor(
    private apiService: ApiService,
    private calendarService: CalendarService,
    public translate: TranslateService,
    private signalRService: SignalRService,
    public dialog: MatDialog,
  ) { }

  ngOnInit() {
    this.getNotes();
    this.calendarService.onNoteFinishEditing$.pipe(takeUntil(this.unsubscribe)).subscribe(
      note => this.onFinishEditing()
    );
    this.signalRService.onUpdate$.pipe(takeUntil(this.unsubscribe)).subscribe(
      calendaCode => {
        if (this.calendarCode == calendaCode) {
          this.getNotes();           
        }
    });
  }

  getNotes() {
    this.isLoading = true;
    this.apiService.getCalendarNotes(this.calendarCode, null, this.pageInfo).subscribe(
      result => {
        this.notes.data = result.notes;
        this.pageInfo.totalCount = result.totalCount;
        this.isLoading = false;                
    });    
  }

  selectNote(note: Note) {
    this.calendarService.selectNote(this.calendarCode, note);
    this.selectedNoteId = note.id;
  }

  deleteNote(note: Note) {

    const dialogRef = this.dialog.open(DeleteNoteDialogComponent, {
      width: '350px',
      data: { noteName: note.name }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.deleteNote(note.id).subscribe(
          () => {        
            this.calendarService.deleteNote(note); 
            this.getNotes();
            this.signalRService.change(this.calendarCode);
          }
        );
      }
    });
    
  }

  private onFinishEditing() {
    this.getNotes();
  }

  pageChanged(pageEvent: PageEvent) {
    this.pageInfo.pageIndex = pageEvent.pageIndex + 1;
    this.pageInfo.pageSize = pageEvent.pageSize;
    this.getNotes();
  }

  sortNotes(sort: Sort) {
    this.pageInfo.pageIndex = 1;
    this.pageInfo.orderBy = sort.active;
    this.pageInfo.desc = sort.direction == "desc";
    this.getNotes();
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
