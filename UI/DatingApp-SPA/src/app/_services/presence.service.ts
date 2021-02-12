import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { AlertyfyService } from './alertify.service';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  huburl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUserSource.asObservable();

  constructor(private toast: AlertyfyService) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.huburl + 'presence', {
        accessTokenFactory: () => localStorage.getItem('token') // user.token
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection
      .start()
      .catch(err => console.log(err));

      this.hubConnection.on('UserIsOnline', username => {
        this.toast.success(username + ' has connected.');
      });

      this.hubConnection.on('UserIsOffline', username => {
        this.toast.info(username + ' has disconnected.');
      });

      this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
        this.onlineUserSource.next(usernames);
      });

      this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
        this.toast.info(knownAs + ' has sent you a message.');
      });
  }

  stopHubConnection() {
    this.hubConnection.stop().catch(er => console.log(er));
  }
}
