import { Injectable } from '@angular/core';
import { UserService } from '../security/user.service';
import { HttpHandler, HttpRequest, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

@Injectable()
export class HeadersInterceptor implements HttpInterceptor {

    constructor(
        private readonly _userService: UserService
    ) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const user = this._userService.getUser();
        let headers = req.headers;

        if (user && user.accessToken) {
            headers = headers.set('Authorization', 'Bearer ' + user.accessToken);
        }

        if (!(req.body instanceof FormData)) {
            headers = headers.set('Content-Type', 'application/json');
        }

        const cloneReq = req.clone({ headers });

        return next.handle(cloneReq);
    }
}
