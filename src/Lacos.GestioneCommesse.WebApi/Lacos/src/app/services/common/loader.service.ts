import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class LoaderService {

    private _pendingRequests = 0;
    private readonly _onNewRequest = new Subject<void>();
    private readonly _onRequestsCompleted = new Subject<void>();

    readonly onNewRequest = this._onNewRequest.asObservable();
    readonly onRequestsCompleted = this._onRequestsCompleted.asObservable();

    newRequest() {
        this._pendingRequests++;
        this._onNewRequest.next();
    }

    requestCompleted() {
        if (this._pendingRequests > 0) {
            this._pendingRequests--;
        } else {
            this._pendingRequests = 0;
        }

        if (!this._pendingRequests) {
            this._onRequestsCompleted.next();
        }
    }

}
