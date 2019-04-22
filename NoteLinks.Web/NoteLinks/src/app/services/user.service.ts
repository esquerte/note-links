import { Injectable } from '@angular/core';
import * as moment from 'moment';

import { ApiService } from './api.service';
import { CreateUserModel } from '../models/create-user-model';
import { InteractionService } from './interaction.service';
import { AuthenticationModel } from '../models/authentication-model';
import { AuthenticationResult } from '../models/authentication-result';
import { DeleteUserModel } from '../models/delete-user-model';
import { Calendar } from '../models/calendar';
import { Subject, Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private userLoggedInSubject = new Subject<string>();
  onUserLoggedIn$ = this.userLoggedInSubject.asObservable();

  private userLoggedOutSubject = new Subject();
  onUserLoggedOut$ = this.userLoggedOutSubject.asObservable();

  private followCalendarSubject = new Subject();
  onFollowCalendar$ = this.followCalendarSubject.asObservable();

  private calendarFollowedSubject = new Subject();
  onCalendarFollowed$ = this.calendarFollowedSubject.asObservable();

  private unfollowCalendarSubject = new Subject();
  onUnfollowCalendar$ = this.unfollowCalendarSubject.asObservable();

  private calendarUnfollowedSubject = new Subject();
  onCalendarUnfollowed$ = this.calendarUnfollowedSubject.asObservable();

  constructor(
    private apiService: ApiService,
    private interactionService: InteractionService,
  ) { }

  create(createModel: CreateUserModel) {
    this.apiService.createUser(createModel).subscribe(
      () => {
        let authModel = new AuthenticationModel();
        authModel.email = createModel.email;
        authModel.password = createModel.password;
        this.login(authModel);
      },
      error => {
        this.interactionService.handleError(error);
      }
    );
  }

  login(authModel: AuthenticationModel) {
    this.apiService.getAuthenticationToken(authModel).subscribe(
      (result: AuthenticationResult) => {

        const expiresAt = moment().add(result.expirationTime, "minutes");

        localStorage.setItem('accessToken', result.accessToken);
        localStorage.setItem('displayName', result.displayName);
        localStorage.setItem("expiresAt", JSON.stringify(expiresAt.valueOf()));

        this.userLoggedInSubject.next(result.displayName);

      },
      error => {
        this.interactionService.handleError(error);
      }
    );
  }

  delete(deleteModel: DeleteUserModel) {
    this.apiService.deleteUser(deleteModel).subscribe(
      () => this.logout(),
      error => {
        this.interactionService.handleError(error);
      }
    );
  }

  getCalendars(): Observable<any> {
    return this.apiService.getUserCalendars();
  }

  followCalendar() {
    this.followCalendarSubject.next();
  }

  unfollowCalendar() {
    this.unfollowCalendarSubject.next();
  }

  calendarFollowed() {
    this.calendarFollowedSubject.next();
  }

  calendarUnfollowed() {
    this.calendarUnfollowedSubject.next();
  }

  getDisplayName(): string {
    if (this.isLoggedIn()) {
      return localStorage.getItem("displayName");
    }
  }

  logout() {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("expiresAt");
    localStorage.removeItem("displayName");
    this.userLoggedOutSubject.next();
  }

  isLoggedIn(): boolean {
    return moment().isBefore(this.getExpiration());
  }

  getExpiration(): moment.Moment {
    const expiresAt = localStorage.getItem("expiresAt");
    return moment(JSON.parse(expiresAt));
  }

}
