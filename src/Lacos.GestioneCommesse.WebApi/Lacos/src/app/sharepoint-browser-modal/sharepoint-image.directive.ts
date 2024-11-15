import { Directive, HostBinding, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { SharepointService } from '../services/sharepoint/sharepoint.service';
import { tap } from 'rxjs';

@Directive({ selector: '[sharepointImage]' })
export class SharepointImageDirective extends BaseComponent implements OnChanges {

    @Input('src')
    url: string;

    @HostBinding('attr.src')
    src: string;

    constructor(
        private readonly _service: SharepointService
    ) {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['url'] && this.url) {
            this._getSrc();
        }
    }
    private _getSrc() {
        this._subscriptions.push(
            this._service.getFileContentUrl(this.url)
                .pipe(
                    tap(e => this.src = e)
                )
                .subscribe()
        );
    }

}