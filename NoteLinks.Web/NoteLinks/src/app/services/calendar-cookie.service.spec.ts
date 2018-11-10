import { TestBed } from '@angular/core/testing';

import { CalendarCookieService } from './calendar-cookie.service';

describe('CalendarCookieService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CalendarCookieService = TestBed.get(CalendarCookieService);
    expect(service).toBeTruthy();
  });
});
