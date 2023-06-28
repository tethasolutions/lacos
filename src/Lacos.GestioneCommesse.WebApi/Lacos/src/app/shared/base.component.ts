import { OnDestroy, Directive } from '@angular/core';
import { Subscription } from 'rxjs';

@Directive()
export abstract class BaseComponent implements OnDestroy {

    protected readonly _subscriptions: Subscription[] = [];

    readonly console = window.console;

    ngOnDestroy() {
        this._subscriptions
            .forEach(e => e.unsubscribe());
    }

}
