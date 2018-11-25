import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
  name: 'customDate'
})
export class CustomDatePipe implements PipeTransform {

  constructor() {}

  transform(value: any, currentLang: string): any {
    if (!value) 
      return "";
    let date = moment(value);
    if (date.hour() == 0 && date.minute() == 0)
      return date.format(currentLang == "en" ? "MM/DD/YYYY" : "DD.MM.YYYY");
    return date.format(currentLang == "en" ? "MM/DD/YYYY hh:mm a" : "DD.MM.YYYY HH:mm");
  }

}
