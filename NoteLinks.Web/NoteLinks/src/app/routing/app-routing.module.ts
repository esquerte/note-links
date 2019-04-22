import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CalendarComponent } from '../calendar/calendar.component';
import { UserSignUpComponent } from '../user-sign-up/user-sign-up.component';
import { UserLogInComponent } from '../user-log-in/user-log-in.component';
import { UserDeleteComponent } from '../user-delete/user-delete.component';

const routes: Routes = [
  { path: 'calendars/:code', component: CalendarComponent },
  { path: 'calendars', component: CalendarComponent },
  { path: 'users/signup', component: UserSignUpComponent },
  { path: 'users/login', component: UserLogInComponent },
  { path: 'users/delete', component: UserDeleteComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
