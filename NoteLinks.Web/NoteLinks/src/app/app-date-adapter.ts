import { Injectable } from '@angular/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';
import { DateFormatService } from './services/date-format.service';

@Injectable()
export class AppDateAdapter extends MomentDateAdapter {

  constructor(
    private translate: TranslateService,
    private dateFormatService: DateFormatService
  ) {
    super('en-US');
  }

  public format(date: moment.Moment, displayFormat: string): string {
    const format = this.dateFormatService.getDateFormat();
    return date.locale(this.translate.currentLang).format(format);
  }
}