import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';
import { PageInfo } from '../models/page-info';
import { Filter } from '../models/filter';

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

  getCalendarNotes(calendarCode: string, filters: Filter[], pageInfo: PageInfo): Observable<any> {
    const url = `${this.serviceUrl}/notes/${calendarCode}`;
    let httpParams = this.buildParamsForGetNotes(filters, pageInfo);
    return this.http.get(url, { params: httpParams })
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

  private buildParamsForGetNotes(filters: Filter[], pageInfo: PageInfo): HttpParams {

    let httpParams = new HttpParams();

    if (filters) {
      filters.forEach((filter, index) => {
        httpParams = httpParams
          .append("filters[" + index + "].Field", filter.field)
          .append("filters[" + index + "].Operator", filter.operator)
          .append("filters[" + index + "].Value", filter.value);
      });      
    }

    if (pageInfo) {
      httpParams = httpParams
        .set("pageIndex", pageInfo.pageIndex.toString())
        .set("pageSize", pageInfo.pageSize.toString())
        .set("orderBy", pageInfo.orderBy)
        .set("desc", pageInfo.desc.toString());      
    }

    return httpParams;
  }

  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }
}
