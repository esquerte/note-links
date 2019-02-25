import { CalendarDateFormatter, DateFormatterParams } from 'angular-calendar';
import * as moment from 'moment';

import { DateFormatService } from '../services/date-format.service';

export class CustomDateFormatter extends CalendarDateFormatter {

  public monthViewColumnHeader({ date, locale }: DateFormatterParams): string {
    return new DateFormatService().getDayOfWeekString(moment(date).day(), locale);
  }
  public monthViewDayNumber({ date, locale }: DateFormatterParams): string {
    return moment(date).date().toString();
  }

}