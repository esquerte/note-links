import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class CalendarService {

  private serviceUrl = 'http://localhost/api';

  constructor(private http: HttpClient) { }

  getCalendar(code: string): Observable<Calendar> {
    const url = `${this.serviceUrl}/calendars/${code}`;
    return this.http.get<Calendar>(url)
    .pipe<Calendar>(
      catchError(this.handleError<Calendar>(`getCalendar code=${code}`))
    );
  }

  updateCalendar(calendar: Calendar): Observable<any> {
    const url = `${this.serviceUrl}/calendars`;
    return this.http.put<Calendar>(url, calendar, httpOptions);
  }

  createCalendar(calendar: Calendar): Observable<any> {
    const url = `${this.serviceUrl}/calendars`;
    var createModel:any = {
      name: calendar.name
    };
    return this.http.post(url, createModel, httpOptions);
  }

  deleteCalendar(code: string): Observable<any> {
    const url = `${this.serviceUrl}/calendars/${code}`;
    return this.http.delete(url, httpOptions);
  }

  getCalendarNotes(code: string): Observable<Note[]> {
    const url = `${this.serviceUrl}/notes/${code}`;
    return this.http.get<Note[]>(url)
    .pipe<Note[]>(
      catchError(this.handleError<Note[]>(`getCalendarNotes code=${code}`, []))
    );
  }

  updateNote(note: Note): Observable<any> {
    const url = `${this.serviceUrl}/notes`;
    return this.http.put<Note>(url, note, httpOptions);
  }

  createNote(calendarCode: string, note: Note): Observable<any> {
    const url = `${this.serviceUrl}/notes`;
    var createModel: any = {
      calendarCode: calendarCode, 
      name: note.name, 
      text: note.text, 
      fromDate: note.fromDate, 
      toDate: note.toDate
    };
    return this.http.post(url, createModel, httpOptions);
  }

  deleteNote(id: number): Observable<any> {
    const url = `${this.serviceUrl}/notes/${id}`;
    return this.http.delete(url, httpOptions);
  }

  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }
}
