import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DateFormatService {

  private daysOfWeekRu: { [number: number] : string; } = {};
  private daysOfWeekDefault: { [number: number] : string; } = {};

  constructor() { 

    this.daysOfWeekRu[0] = "вс";
    this.daysOfWeekRu[1] = "пн";
    this.daysOfWeekRu[2] = "вт";
    this.daysOfWeekRu[3] = "ср";
    this.daysOfWeekRu[4] = "чт";
    this.daysOfWeekRu[5] = "пт";
    this.daysOfWeekRu[6] = "сб";

    this.daysOfWeekDefault[0] = "Su";
    this.daysOfWeekDefault[1] = "Mo";
    this.daysOfWeekDefault[2] = "Tu";
    this.daysOfWeekDefault[3] = "We";
    this.daysOfWeekDefault[4] = "Th";
    this.daysOfWeekDefault[5] = "Fr";
    this.daysOfWeekDefault[6] = "Sa";

  }

  getDayOfWeekString(dayOfWeek: number, currentLang: string): string {

    let result: string;

    switch (currentLang) {
      case "ru": {
        result = this.daysOfWeekRu[dayOfWeek]
        break;
      }
      default: {
        result = this.daysOfWeekDefault[dayOfWeek]
        break;
      }
    }

    return result;

  }

  getDateFormat(currentLang: string): string {

    let result: string;

    switch (currentLang) {
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

  getTimeFormat(currentLang: string): string {

    let result: string;

    switch (currentLang) {
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