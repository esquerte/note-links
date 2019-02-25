import { Component, OnInit, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

import { Note } from '../models/note';

@Component({
  selector: 'app-note-info',
  templateUrl: './note-info.component.html',
  styleUrls: ['./note-info.component.scss']
})
export class NoteInfoComponent implements OnInit {

  @Input() note: Note;

  constructor(public translate: TranslateService) { 
  }

  ngOnInit() {   
  }

}
