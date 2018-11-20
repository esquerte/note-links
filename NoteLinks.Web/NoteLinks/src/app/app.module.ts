import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule }   from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { MaterialModule } from './app-material.module';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

import { AppComponent } from './app.component';
import { CalendarComponent } from './calendar/calendar.component';
import { NotesListComponent } from './notes-list/notes-list.component';
import { NoteInfoComponent } from './note-info/note-info.component';
import { AppRoutingModule } from './routing/app-routing.module';
import { NoteEditComponent } from './note-edit/note-edit.component';
import { CalendarInfoComponent } from './calendar-info/calendar-info.component';
import { CalendarEditComponent } from './calendar-edit/calendar-edit.component';
import { CalendarCookieService } from './services/calendar-cookie.service';
import { NoteComponent } from './note/note.component';

@NgModule({
  declarations: [
    AppComponent,
    CalendarComponent,
    NotesListComponent,
    NoteInfoComponent,
    NoteEditComponent,
    CalendarInfoComponent,
    CalendarEditComponent,
    NoteComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MaterialModule,
    NgxMaterialTimepickerModule.forRoot(),
  ],
  providers: [
    CookieService, 
    CalendarCookieService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
