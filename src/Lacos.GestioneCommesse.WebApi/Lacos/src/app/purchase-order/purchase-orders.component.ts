import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
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
import { OperatorModel } from '../shared/models/operator.model';
import { User } from '../services/security/models';
import { UserService } from '../services/security/user.service';
import { OperatorsService } from '../services/operators.service';
import { JobsService } from '../services/jobs/jobs.service';
import { JobModalComponent } from '../jobs/job-modal.component';
import { JobsAttachmentsModalComponent } from '../jobs/jobs-attachments-modal.component';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';

@Component({
    selector: 'app-purchase-orders',
    templateUrl: 'purchase-orders.component.html'
})
export class PurchaseOrdersComponent extends BaseComponent implements OnInit {

    @ViewChild('purchaseOrderModal', { static: true }) purchaseOrderModal: PurchaseOrderModalComponent;
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;
    @ViewChild('jobModal', { static: true }) jobModal: JobModalComponent;
    @ViewChild('jobsAttachmentsModal', { static: true }) jobsAttachmentsModal: JobsAttachmentsModalComponent;
    user: User;
    currentOperator: OperatorModel;
    screenWidth: number;
    
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
        sort: [{ field: 'date', dir: 'desc' },{ field: 'code', dir: 'desc' }]
    };

    private _jobId: number;
    private cellArgs: CellClickEvent;

    readonly purchaseOrderStatusNames = purchaseOrderStatusNames;

    constructor(
        private readonly _service: PurchaseOrdersService,
        private readonly _messageBox: MessageBoxService,
        private readonly _route: ActivatedRoute,
        private readonly _user: UserService,
        private readonly _operatorsService: OperatorsService,
        private readonly _storageService: StorageService,
        private readonly _customerService: CustomerService,
        private readonly _jobService: JobsService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this._subscribeRouteParams();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
        this.updateScreenSize();
      }
    
      @HostListener('window:resize', ['$event'])
      onResize(event: Event): void {
        this.updateScreenSize();
      }
    
      private updateScreenSize(): void {
        this.screenWidth = window.innerWidth -44;
        if (this.screenWidth > 1876) this.screenWidth = 1876;
        if (this.screenWidth < 1400) this.screenWidth = 1400;     
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
        const purchaseOrder = new PurchaseOrder(0, null, today.getFullYear(), today, null, null, PurchaseOrderStatus.Pending, this._jobId, null, null, this.currentOperator.id, [], [], [], [], []);
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

    protected _getCurrentOperator(userId: number) {
        this._subscriptions.push(
            this._operatorsService.getOperatorByUserId(userId)
                .pipe(
                    tap(e => this.currentOperator = e)
                )
                .subscribe()
        );
    }
    
    openCustomer(customerId: number): void {
        this._subscriptions.push(
            this._customerService.getCustomer(customerId)
                .pipe(
                    map(e => {
                        return Object.assign(new CustomerModel(), e);
                    }),
                    switchMap(e => this.customerModal.open(e)),
                    filter(e => e),
                    map(() => this.customerModal.options),
                    switchMap(e => this._customerService.updateCustomer(e, customerId)),
                    map(() => this.customerModal.options),
                    tap(e => this._messageBox.success(`Cliente ${e.name} aggiornato`)),
                )
                .subscribe()
        );
    }
    
    openJob(jobId: number): void {
        this._subscriptions.push(
            this._jobService.get(jobId)
            .pipe(
                switchMap(e => this.jobModal.open(e)),
                filter(e => e),
                switchMap(() => this._jobService.update(this.jobModal.options)),
                tap(e => this._read())
            )
            .subscribe()
        );
    }

    openAttachments(jobId: number, purchaseOrderId: number) {
        this._subscriptions.push(
            this.jobsAttachmentsModal.open([0, 0, purchaseOrderId])
                .pipe(
                    filter(e => e)
                )
                .subscribe()
        );
    }
}
