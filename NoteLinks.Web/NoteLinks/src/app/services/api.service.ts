import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { retry, catchError } from 'rxjs/operators';

import { Calendar } from '../models/calendar';
import { Note } from '../models/note';
import { PageInfo } from '../models/page-info';
import { Filter } from '../models/filter';
import { ApiError } from '../models/api-error';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  withCredentials: true
};

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private serviceUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCalendar(code: string): Observable<Calendar> {
    const url = `${this.serviceUrl}/calendars/${code}`;
    return this.http.get<Calendar>(url, { withCredentials: true }).pipe(
      retry(3),
      catchError(this.handleError)
    );
  }

  updateCalendar(calendar: Calendar): Observable<any> {
    const url = `${this.serviceUrl}/calendars`;
    return this.http.put<Calendar>(url, calendar, httpOptions).pipe(
      retry(3),
      catchError(this.handleError)
    );
  }

  createCalendar(calendar: Calendar): Observable<any> {
    const url = `${this.serviceUrl}/calendars`;
    var createModel: any = {
      name: calendar.name
    };
    return this.http.post(url, createModel, httpOptions).pipe(
      retry(3),
      catchError(this.handleError)
    );
  }

  deleteCalendar(code: string): Observable<any> {
    const url = `${this.serviceUrl}/calendars/${code}`;
    return this.http.delete(url, httpOptions).pipe(
      retry(3),
      catchError(this.handleError)
    );
  }

  getCalendarNotes(calendarCode: string, filters: Filter[], pageInfo: PageInfo): Observable<any> {
    const url = `${this.serviceUrl}/notes/${calendarCode}`;
    let httpParams = this.buildParamsForGetNotes(filters, pageInfo);
    return this.http.get(url, { params: httpParams }).pipe(
      retry(3),
      catchError(this.handleError)
    );
  }

  updateNote(note: Note): Observable<any> {
    const url = `${this.serviceUrl}/notes`;
    return this.http.put<Note>(url, note, httpOptions).pipe(
      retry(3),
      catchError(this.handleError)
    );
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
    return this.http.post(url, createModel, httpOptions).pipe(
      retry(3),
      catchError(this.handleError)
    );
  }

  deleteNote(id: number): Observable<any> {
    const url = `${this.serviceUrl}/notes/${id}`;
    return this.http.delete(url, httpOptions).pipe(
      retry(3),
      catchError(this.handleError)
    );
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

  private handleError(error: HttpErrorResponse) {
      
    let message: string;

    if (error.error instanceof ErrorEvent) {

      message = "A client-side or network error occurred."
      console.error(message, error.error.message);

    } else {

      let apiError: ApiError = error.error;        
      message = `Error ${error.status}. ${apiError.message} \n`;

      if (!environment.production) {
        let errors: string = "";
        Object.keys(apiError.errors).forEach(key => errors += `${key}: ${apiError.errors[key]} \n`);
        message += errors;
      }

      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
        
    }

    return throwError(message);
  };

}
