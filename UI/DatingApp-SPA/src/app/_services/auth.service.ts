import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'Auth/';
  jwtHelperService = new JwtHelperService();
  decodeToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/16.1 user.png.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient,
    private presence: PresenceService) { }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map((reponse: any) => {
          const user = reponse;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            this.decodeToken = this.jwtHelperService.decodeToken(user.token);
            this.currentUser = user.user;
            this.currentUser.roles = [];
            const roles = this.decodeToken.role;
            Array.isArray(roles) ? this.currentUser.roles = roles : this.currentUser.roles.push(roles);
            this.changeMemberPhoto(this.currentUser.photoUrl);
            this.presence.createHubConnection(user);
          }
        })
      );
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'Register', user);

  }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelperService.isTokenExpired(token);
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currentUser = null;
    this.decodeToken = null;
    this.presence.stopHubConnection();
  }
}
