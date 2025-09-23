import {CanActivate, Router} from '@angular/router';
import {Injectable} from '@angular/core';
import {AuthService} from './auth.service';

@Injectable(
  {
    providedIn: 'root'
  }
)
export class AuthGuard implements CanActivate {

  constructor(private router : Router, private faceSnapService : AuthService) {

  }


  canActivate(): boolean {
    const token = null;
    if (token) {
      return true;
    } else {

      this.router.navigateByUrl('/login');
      return false;
    }
  }
}
