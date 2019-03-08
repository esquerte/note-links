import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HttpClient, HttpClientXsrfModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { MaterialModule } from './app-material.module';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { DatePipe } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import * as moment from 'moment';

import { DateAdapter } from '@angular/material';

import { adapterFactory } from 'angular-calendar/date-adapters/moment';
import {
  CalendarDateFormatter,
  CalendarModule,
  CalendarMomentDateFormatter,
  DateAdapter as CalendarDateAdapter,
} from 'angular-calendar';

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
import { SignalRService } from './services/signal-r.service';
import { DatePickerComponent } from './date-picker/date-picker.component'
import { AppDateAdapter } from './app-date-adapter';
import { DeleteCalendarDialogComponent } from './delete-calendar-dialog/delete-calendar-dialog.component';
import { DeleteNoteDialogComponent } from './delete-note-dialog/delete-note-dialog.component';
import { DatesValidatorDirective } from './note-edit/dates-validator.directive';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function momentAdapterFactory() {
  return adapterFactory(moment);
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
    CustomDatePipe,
    DatePickerComponent,
    DeleteCalendarDialogComponent,
    DeleteNoteDialogComponent,
    DatesValidatorDirective,
  ],
  entryComponents: [
    DeleteCalendarDialogComponent,
    DeleteNoteDialogComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    HttpClientXsrfModule,
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
    BrowserAnimationsModule,
    CalendarModule.forRoot(
      {
        provide: CalendarDateAdapter,
        useFactory: momentAdapterFactory
      },
      {
        dateFormatter: {
          provide: CalendarDateFormatter,
          useClass: CalendarMomentDateFormatter
        }
      }
    ),
  ],
  providers: [
    CookieService,
    CalendarCookieService,
    DatePipe,
    CustomDatePipe,
    SignalRService,
    {
      provide: DateAdapter,
      useClass: AppDateAdapter
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
