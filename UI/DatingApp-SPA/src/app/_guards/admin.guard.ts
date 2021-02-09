import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AlertyfyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class AdminGuard implements CanActivate {

  constructor(private authService: AuthService,
    private alertify: AlertyfyService) {}

  canActivate(): Observable<boolean> {
    const currentUser = of(this.authService.currentUser);
    return currentUser.pipe(
      map(user => {
        if (this.authService.currentUser.roles.includes('Admin')
        ||  this.authService.currentUser.roles.includes('Moderator')) {
          return true;
        } else {
          this.alertify.error('you can not enter this area.');
        }
      })
    );
  }
}
