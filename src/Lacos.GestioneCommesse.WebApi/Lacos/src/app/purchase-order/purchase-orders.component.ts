import { Component, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { PurchaseOrderModalComponent, PurchaseOrderModalOptions } from './purchase-order-modal.component';
import { IPurchaseOrderReadModel, PurchaseOrder, PurchaseOrderStatus, purchaseOrderStatusNames } from '../services/purchase-orders/models';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { getToday } from '../services/common/functions';
import { ActivatedRoute, Params } from '@angular/router';
import { StorageService } from '../services/common/storage.service';

@Component({
    selector: 'app-purchase-orders',
    templateUrl: 'purchase-orders.component.html'
})
export class PurchaseOrdersComponent extends BaseComponent implements OnInit {

    @ViewChild('purchaseOrderModal', { static: true })
    purchaseOrderModal: PurchaseOrderModalComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
                this._buildStatusFilter(),
                this._buildJobIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'date', dir: 'desc' }]
    };

    private _jobId: number;
    private cellArgs: CellClickEvent;

    readonly purchaseOrderStatusNames = purchaseOrderStatusNames;

    constructor(
        private readonly _service: PurchaseOrdersService,
        private readonly _messageBox: MessageBoxService,
        private readonly _route: ActivatedRoute,
        private readonly _storageService: StorageService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this._subscribeRouteParams();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._saveState();
        this._read();
    }

    private _resumeState() {
        const savedState = this._storageService.get<State>(window.location.hash, true);
        if (savedState == null) return;
        this.gridState = savedState;
    }

    private _saveState() {
        this._storageService.save(this.gridState,window.location.hash,true);
    }
    
    askRemove(purchaseOrder: IPurchaseOrderReadModel) {
        const text = `Sei sicuro di voler rimuovere l'ordine ${purchaseOrder.code}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(purchaseOrder.id)),
                    tap(() => this._afterRemoved(purchaseOrder))
                )
                .subscribe()
        );
    }

    create() {
        const today = getToday();
        const purchaseOrder = new PurchaseOrder(0, null, today.getFullYear(), today, null, null, PurchaseOrderStatus.Pending, this._jobId, null, null, null, null, [], []);
        const options = new PurchaseOrderModalOptions(purchaseOrder);

        this._subscriptions.push(
            this.purchaseOrderModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(purchaseOrder)),
                    tap(() => this._afterPurchaseOrderCreated())
                )
                .subscribe()
        );
    }

    edit(purchaseOrder: IPurchaseOrderReadModel) {
        this._subscriptions.push(
            this._service.get(purchaseOrder.id)
                .pipe(
                    map(e => new PurchaseOrderModalOptions(e)),
                    switchMap(e => this.purchaseOrderModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.purchaseOrderModal.options.purchaseOrder)),
                    tap(() => this._afterPurchaseOrderUpdated())
                )
                .subscribe()
        );
    }

    onDblClick(): void {
        if (!this.cellArgs.isEdited) {
            this._subscriptions.push(
                this._service.get(this.cellArgs.dataItem.id)
                    .pipe(
                        map(e => new PurchaseOrderModalOptions(e)),
                        switchMap(e => this.purchaseOrderModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._service.update(this.purchaseOrderModal.options.purchaseOrder)),
                        tap(() => this._afterPurchaseOrderUpdated())
                    )
                    .subscribe()
            );
        }
    }

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    readonly rowCallback = (context: RowClassArgs) => {
        const purchaseOrder = context.dataItem as IPurchaseOrderReadModel;

        switch (true) {
            case purchaseOrder.status === PurchaseOrderStatus.Completed:
                return { 'purchase-order-completed': true };
            case purchaseOrder.status === PurchaseOrderStatus.Pending:
                return { 'purchase-order-pending': true };
            case purchaseOrder.status === PurchaseOrderStatus.Ordered:
                return { 'purchase-order-ordered': true };
            case purchaseOrder.status === PurchaseOrderStatus.Withdrawn:
                return { 'purchase-order-withdrawn': true };
            case purchaseOrder.status === PurchaseOrderStatus.Canceled:
                return { 'purchase-order-canceled': true };
            default:
                return {};
        }
    };

    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    private _afterPurchaseOrderCreated() {
        this._messageBox.success('Ordine creato.')

        this._read();
    }

    private _afterPurchaseOrderUpdated() {
        this._messageBox.success('Ordine aggiornato.')

        this._read();
    }

    private _afterRemoved(purchaseOrder: IPurchaseOrderReadModel) {
        const text = `Ordine ${purchaseOrder.code} rimosso.`;

        this._messageBox.success(text);

        this._read();
    }

    private _buildJobIdFilter() {
        const that = this;

        return {
            field: 'jobId',
            get operator() {
                return that._jobId
                    ? 'eq'
                    : 'isnotnull'
            },
            get value() {
                return that._jobId;
            }
        };
    }

    private _buildStatusFilter() {
        const that = this;

        return {
            get field() {
                return that._jobId
                    ? 'id'
                    : undefined
            },
            get operator() {
                return that._jobId
                    ? 'isnotnull'
                    : undefined
            },
            get filters() {
                return that._jobId
                    ? undefined
                    : [PurchaseOrderStatus.Pending, PurchaseOrderStatus.Ordered]
                        .map(e => ({ field: 'status', operator: 'eq', value: e }))
            },
            logic: 'or'
        };
    }

    private _subscribeRouteParams() {
        this._route.queryParams
            .pipe(
                tap(e => this._setParams(e))
            )
            .subscribe();
    }

    private _setParams(params: Params) {
        this._jobId = isNaN(+params['jobId']) ? null : +params['jobId'];
        this._read();
    }


}
