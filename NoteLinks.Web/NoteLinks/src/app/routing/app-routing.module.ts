import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CalendarComponent } from '../calendar/calendar.component';
import { CalendarEditComponent } from '../calendar-edit/calendar-edit.component';

const routes: Routes = [
  { path: 'edit/:code', component: CalendarEditComponent },
  { path: 'edit', component: CalendarEditComponent },
  { path: 'calendars/:code', component: CalendarComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
