import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { FormsModule }   from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { MaterialModule } from './app-material.module';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { DatePipe } from '@angular/common';

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
import { CustomDatePipe } from './pipes/custom-date.pipe';
import { SignalRService } from './services/signal-r.service'

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    CalendarComponent,
    NotesListComponent,
    NoteInfoComponent,
    NoteEditComponent,
    CalendarInfoComponent,
    CalendarEditComponent,
    NoteComponent,
    CustomDatePipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MaterialModule,
    NgxMaterialTimepickerModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
          provide: TranslateLoader,
          useFactory: (createTranslateLoader),
          deps: [HttpClient]
      }
  }),
  ],
  providers: [
    CookieService, 
    CalendarCookieService,
    DatePipe,
    CustomDatePipe,
    SignalRService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
