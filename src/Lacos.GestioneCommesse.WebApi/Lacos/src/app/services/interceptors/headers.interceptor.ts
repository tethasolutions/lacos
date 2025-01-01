import { Injectable } from '@angular/core';
import { UserService } from '../security/user.service';
import { HttpHandler, HttpRequest, HttpEvent, HttpInterceptor, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { SharepointService } from '../sharepoint/sharepoint.service';

@Injectable()
export class HeadersInterceptor implements HttpInterceptor {

    constructor(
        private readonly _userService: UserService,
        private readonly _sharepointService: SharepointService
    ) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const user = this._userService.getUser();
        const isSharepointApiCall = req.url.includes('sharepoint');
        const isSharepointTokenApiCall = req.url.includes('token');

        let headers = req.headers;

        if (isSharepointApiCall && !isSharepointTokenApiCall) {
            headers = headers.set('Authorization', 'Bearer ' + this._sharepointService.graphApi.accessToken);
            headers = headers.set('Content-Type', 'application/json;odata=verbose');
        } else {
            if (user && user.accessToken) {
                headers = headers.set('Authorization', 'Bearer ' + user.accessToken);
            }

            if (!(req.body instanceof FormData)) {
                headers = headers.set('Content-Type', 'application/json');
            }
        }

        const cloneReq = req.clone({ headers });

        return next.handle(cloneReq);
    }
}
