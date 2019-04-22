import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CreateUserModel } from '../models/create-user-model';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-sign-up',
  templateUrl: './user-sign-up.component.html',
  styleUrls: ['./user-sign-up.component.scss']
})
export class UserSignUpComponent implements OnInit, OnDestroy {

  createModel = new CreateUserModel();

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

  create(){
    this.userService.create(this.createModel);
  }
  
  cancel() {
    this.location.back();
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
