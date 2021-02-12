import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertyfyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { MessageService } from '../_services/message.service';
import { UserService } from '../_services/user.service';

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
pageNumber = 1;
pageSize = 5;
    messageContainer = 'Unread';

    constructor(private messageService: MessageService,
        private authService: AuthService,
        private router: Router,
        private alertify: AlertyfyService
    ) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        return this.messageService.getMessages(
             this.pageNumber, this.pageSize, this.messageContainer).pipe(
            catchError(error => {
                console.log(error);
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
