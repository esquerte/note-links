import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';
import { PageInfo } from '../models/page-info';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private serviceUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCalendar(code: string): Observable<Calendar> {
    const url = `${this.serviceUrl}/calendars/${code}`;
    return this.http.get<Calendar>(url);
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

  getCalendarNotes(calendarCode: string, pageInfo: PageInfo): Observable<any> {
    const url = `${this.serviceUrl}/notes/${calendarCode}`;
    return this.http.get<Note[]>(url, { 
      params: {
        pageIndex: pageInfo.pageIndex.toString(),
        pageSize: pageInfo.pageSize.toString(),
        orderBy: pageInfo.orderBy,
        desc: pageInfo.desc.toString()
      }
    })
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
