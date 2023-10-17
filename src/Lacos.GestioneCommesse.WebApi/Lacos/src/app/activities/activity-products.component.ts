import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, switchMap, tap } from 'rxjs';
import { IActivityProductReadModel } from '../services/activity-products/models';
import { ApiUrls } from '../services/common/api-urls';
import { ActivityProductsService } from '../services/activity-products/activity-products.service';

@Component({
    selector: 'app-activity-products',
    templateUrl: 'activity-products.component.html'
})
export class ActivityProductsComponent extends BaseComponent implements OnChanges {

    @Input()
    activityId: number;

    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 10,
        filter: {
            filters: [
                this._buildActivityIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: []
    };

    constructor(
        private readonly _service: ActivityProductsService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['activityId'] && this.activityId) {
            this._read();
        }
    }

    refresh() {
        this._read();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    askRemove(product: IActivityProductReadModel) {
        const text = `Sei sicuro di voler rimuovere ${product.name} dall'attività?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(product.id)),
                    tap(() => this._afterRemoved(product))
                )
                .subscribe()
        );
    }

    askDuplicate(product: IActivityProductReadModel) {
        const text = `Sei sicuro di voler associare un altro ${product.name} all'attività?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.duplicate(product.id)),
                    tap(() => this._afterDuplicated(product))
                )
                .subscribe()
        );
    }

    private _afterRemoved(product: IActivityProductReadModel) {
        const text = `${product.name} rimosso dall'attività.`;

        this._messageBox.success(text);

        this._read();
    }

    private _afterDuplicated(product: IActivityProductReadModel) {
        const text = `${product.name} associato all'attività.`;

        this._messageBox.success(text);

        this._read();
    }

    private _buildActivityIdFilter() {
        const that = this;

        return {
            field: 'activityId',
            operator: 'eq',
            get value() { return that.activityId; }
        };
    }
}
