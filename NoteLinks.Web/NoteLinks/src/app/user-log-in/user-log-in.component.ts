import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { UserService } from '../services/user.service';
import { AuthenticationModel } from '../models/authentication-model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-log-in',
  templateUrl: './user-log-in.component.html',
  styleUrls: ['./user-log-in.component.scss']
})
export class UserLogInComponent implements OnInit, OnDestroy {

  authModel = new AuthenticationModel();

  private unsubscribe: Subject<void> = new Subject();

  constructor(
    private location: Location,
    private userService: UserService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.userService.onUserLoggedIn$.pipe(takeUntil(this.unsubscribe)).subscribe(
      () => this.location.back() //this.router.navigate(["/"])
    );
  }

  authenticate(){
    this.userService.login(this.authModel);   
  }
  
  cancel() {
    //console.log(this.userService.getExpiration().toString());
    this.location.back();
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}
