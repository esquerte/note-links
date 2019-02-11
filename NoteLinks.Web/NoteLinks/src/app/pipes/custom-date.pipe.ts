import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';
import { DateFormatService } from '../services/date-format.service';

@Pipe({
  name: 'customDate'
})
export class CustomDatePipe implements PipeTransform {

  constructor(private dateFormatService: DateFormatService) {}

  transform(value: any, currentLang: string): any {

    if (!value) 
      return "";

    let date = moment(value);

    let dateFormat = this.dateFormatService.getDateFormat(currentLang);
    let timeFormat = this.dateFormatService.getTimeFormat(currentLang);

    let format = date.hour() == 0 && date.minute() == 0 ? dateFormat : dateFormat + " " + timeFormat;

    return date.format(format);
  }

}
