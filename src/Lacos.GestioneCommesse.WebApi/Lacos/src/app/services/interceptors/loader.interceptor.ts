import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { finalize } from 'rxjs/operators';
import { LoaderService } from '../common/loader.service';
import { Observable } from 'rxjs/internal/Observable';

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {

    constructor(
        private readonly _loaderService: LoaderService
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const avoidLoader = req.headers.has('avoid-loader');

        if (avoidLoader) {
            req.headers.delete('avoid-loader');
        } else {
            this._loaderService.newRequest();
        }

        return next.handle(req)
            .pipe(
                finalize(() => !avoidLoader && this._loaderService.requestCompleted())
            );
    }
}
