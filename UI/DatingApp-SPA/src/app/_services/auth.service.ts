import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'Auth/';
  jwtHelperService = new JwtHelperService();
  decodeToken: any;

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map((reponse: any) => {
          const user = reponse;
          if (user) {
            localStorage.setItem('token', user.token);
            this.decodeToken = this.jwtHelperService.decodeToken(user.token);
            console.log(this.decodeToken);
          }
        })
      );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'Register', model);

  }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelperService.isTokenExpired(token);
  }

}
