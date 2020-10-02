import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertyfyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(public authService: AuthService,
     private alertify: AlertyfyService,
     private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl
    .subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Login sucesss.');
    }, error => {

    }, () => {
      this.router.navigate(['/members']);
    });
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  logOut() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.currentUser = null;
    this.authService.decodeToken = null;
    this.router.navigate(['/']);
  }
}
