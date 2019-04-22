import { Directive } from '@angular/core';
import { ValidatorFn, AbstractControl, NG_VALIDATORS, Validator, ValidationErrors, FormGroup } from '@angular/forms';
import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';
import { DateFormatService } from '../services/date-format.service';

export function datesValidator(timeFormat: string): ValidatorFn {
  return (control: FormGroup): ValidationErrors | null => {
    
    let dateError: boolean;

    const fromDate = control.get('fromDate');
    const fromTime = control.get('fromTime');
    const toDate = control.get('toDate');
    const toTime = control.get('toTime');

    if (fromDate && toDate && fromDate.value && toDate.value) {

      if (moment(fromDate.value).isAfter(toDate.value)) {
        dateError = true;
      } else if (moment(fromDate.value).isSame(toDate.value) && (fromTime && toTime && (fromTime.value || toTime.value))) {
        if (!fromTime.value) {
          dateError = true;
        } else if (toTime.value) {
          if (moment(fromTime.value, timeFormat).isAfter(moment(toTime.value, timeFormat))) {
            dateError = true;
          }
        }
      }

    }

    return dateError ? { 'wrongOrder': true } : null;
  };
}

@Directive({
  selector: '[datesOrder]',
  providers: [{ provide: NG_VALIDATORS, useExisting: DatesValidatorDirective, multi: true }]
})
export class DatesValidatorDirective implements Validator {

  constructor(
    private translate: TranslateService,
    private dateFormatService: DateFormatService,
  ){}

  validate(control: AbstractControl): ValidationErrors {
    let timeFormat = this.dateFormatService.getTimeFormat(this.translate.currentLang);
    return datesValidator(timeFormat)(control);
  }
  
}
