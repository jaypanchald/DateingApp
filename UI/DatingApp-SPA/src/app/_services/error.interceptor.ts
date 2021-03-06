import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HTTP_INTERCEPTORS, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertyfyService } from './alertify.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private alertify: AlertyfyService) {}


    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error instanceof HttpErrorResponse) {
                    if (error.status === 401) {
                        return throwError(error.statusText);
                    }
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        console.log(applicationError);
                        return throwError(applicationError);
                    }
                    const serverError = error.error;
                    let modalStateError = '';
                    if (serverError && typeof serverError === 'object') {
                        for (const key in serverError) {
                            if (serverError[key]) {
                                modalStateError += serverError[key] + '\n';
                            }
                        }
                    } else if (serverError && typeof serverError  === 'string') {
                        this.alertify.error(serverError);
                    }

                    return throwError(modalStateError || serverError || 'Server Error');
                }
            })
        );
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
