import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../_models/user';
import { AlertyfyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';

@Injectable()
export class ListsResolver implements Resolve<User[]> {
pageNumber = 1;
pageSize = 5;
likersParam = 'Likers';

    constructor(private userServie: UserService,
        private router: Router,
        private alertify: AlertyfyService
    ) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userServie.getUsers(this.pageNumber, this.pageSize,
            null, this.likersParam).pipe(
            catchError(error => {
                console.log(error);
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
