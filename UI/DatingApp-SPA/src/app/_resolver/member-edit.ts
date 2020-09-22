import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../_models/user';
import { AlertyfyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {

    constructor(private userServie: UserService,
        private router: Router,
        private authService: AuthService,
        private alertify: AlertyfyService
    ) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userServie.getUser(this.authService.decodeToken.nameid).pipe(
            catchError(error => {
                console.log(error);
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
