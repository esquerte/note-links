import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class DateFormatService {

  constructor(private translate: TranslateService) { }

  getDateFormat(currentLang?: string): string {

    let result: string;
    let _currentLang = currentLang ? currentLang : this.translate.currentLang;

    switch (_currentLang) {
      case "en": {
        result = "M/D/YYYY"
        break;
      }
      case "ru": {
        result = "DD.MM.YYYY"
        break;
      }
      default: {
        result = "M/D/YYYY"
        break;
      }
    }

    return result;

  }

  getTimeFormat(currentLang?: string): string {

    let result: string;
    let _currentLang = currentLang ? currentLang : this.translate.currentLang;

    switch (_currentLang) {
      case "en": {
        result = "hh:mm a"
        break;
      }
      case "ru": {
        result = "HH:mm"
        break;
      }
      default: {
        result = "hh:mm a"
        break;
      }
    }

    return result;

  }

}