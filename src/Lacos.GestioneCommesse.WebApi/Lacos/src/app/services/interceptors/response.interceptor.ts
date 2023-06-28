import { Injectable } from '@angular/core';
import { UserService } from '../security/user.service';
import { HttpHandler, HttpRequest, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { Router } from '@angular/router';
import { catchError, finalize } from 'rxjs/operators';
import { throwError, of } from 'rxjs';
import { MessageBoxService } from '../common/message-box.service';

@Injectable()
export class ResponseInterceptor implements HttpInterceptor {

    constructor(
        private readonly _router: Router,
        private readonly _userService: UserService,
        private readonly _messageBox: MessageBoxService
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(
                catchError(e => this._catchError(e)),
                finalize(() => { })
            );
    }

    private _catchError(e: any) {
        if (e instanceof HttpErrorResponse && this._manageHttpError(e)) {
            return of();
        }

        return throwError(() => e);
    }

    private _manageHttpError(error: HttpErrorResponse) {
        switch (error.status) {
            case 401:
                this._userService.hasUser && this._userService.removeUser();
                this._router.navigateByUrl('/login');
                return true;
            case 500:
            case 400:
                this._messageBox.error(error.error);
                return true;
        }

        return false;
    }
}
