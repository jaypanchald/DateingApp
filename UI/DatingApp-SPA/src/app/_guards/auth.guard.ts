import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AlertyfyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private alertify: AlertyfyService, private authsService: AuthService, private router: Router) { }

  canActivate(): boolean {
    if (this.authsService.isLoggedIn()) {
      return true;
    }
    this.alertify.warning('Invalid request, you dont have access');
    this.router.navigate(['/']);
    return false;
  }
}
