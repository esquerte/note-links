import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { UserService } from '../services/user.service';
import { DeleteUserModel } from '../models/delete-user-model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-delete',
  templateUrl: './user-delete.component.html',
  styleUrls: ['./user-delete.component.scss']
})
export class UserDeleteComponent implements OnInit, OnDestroy {

  deleteModel = new DeleteUserModel();

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private location: Location,
    private userService: UserService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.userService.onUserLoggedIn$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.router.navigate(["/"])
    );
  }

  delete(){
    this.userService.delete(this.deleteModel);
  }
  
  cancel() {
    this.location.back();
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
