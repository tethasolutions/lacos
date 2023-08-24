import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, switchMap, tap } from 'rxjs';
import { IInterventionProductReadModel } from '../services/intervention-products/models';
import { ApiUrls } from '../services/common/api-urls';
import { InterventionProductsService } from '../services/intervention-products/intervention-products.service';

@Component({
    selector: 'app-intervention-products',
    templateUrl: 'intervention-products.component.html'
})
export class InterventionProductsComponent extends BaseComponent implements OnChanges {

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
        private readonly _service: InterventionProductsService,
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
                    tap(e => this.data = e),
                    tap(e => this.console.log(e))
                )
                .subscribe()
        );
    }

    askRemove(product: IInterventionProductReadModel) {
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

    private _afterRemoved(product: IInterventionProductReadModel) {
        const text = `${product.name} rimosso dall'attività.`;

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
