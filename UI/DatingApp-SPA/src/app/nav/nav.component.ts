import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertyfyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(public authService: AuthService, private alertify: AlertyfyService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Login sucesss.');
    });
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  logOut() {
    localStorage.removeItem('token');
  }
}
