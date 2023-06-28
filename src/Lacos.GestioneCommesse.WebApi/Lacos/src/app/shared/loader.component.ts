import { Component, HostBinding, OnInit } from '@angular/core';
import { LoaderService } from '../services/common/loader.service';
import { BaseComponent } from './base.component';
import { tap } from 'rxjs';

@Component({
    selector: 'lacos-loader, [lacosLoader]',
    templateUrl: 'loader.component.html'
})
export class LoaderComponent extends BaseComponent implements OnInit {

    @HostBinding('class.hidden')
    hidden = true;

    constructor(
        private readonly _loader: LoaderService
    ) {
        super();
    }

    ngOnInit() {
        this._subscribeLoaderEvents();
    }

    private _subscribeLoaderEvents() {
        this._subscriptions.push(
            this._loader.onNewRequest
                .pipe(
                    tap(() => this.hidden = false)
                )
                .subscribe(),
            this._loader.onRequestsCompleted
                .pipe(
                    tap(() => this.hidden = true)
                )
                .subscribe()
        );
    }

}
