import { MatPaginatorIntl } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';

@Injectable()
export class CustomPaginatorIntl extends MatPaginatorIntl {

  constructor(private translate: TranslateService) {
    super();
  }

  itemsPerPageLabel = this.translate.instant("paginator.itemsPerPageLabel");
  nextPageLabel = this.translate.instant("paginator.nextPageLabel");
  previousPageLabel = this.translate.instant("paginator.previousPageLabel");

}