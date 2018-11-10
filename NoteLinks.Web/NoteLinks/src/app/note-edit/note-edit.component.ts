import { Component, OnInit, Input } from '@angular/core';
import { Note } from '../models/note';

@Component({
  selector: 'app-note-edit',
  templateUrl: './note-edit.component.html',
  styleUrls: ['./note-edit.component.css']
})
export class NoteEditComponent implements OnInit {

  @Input() note: Note;

  constructor() {}

  ngOnInit() {
  }

}
