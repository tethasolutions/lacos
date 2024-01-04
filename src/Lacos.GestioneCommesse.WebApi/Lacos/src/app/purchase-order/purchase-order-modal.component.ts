import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobsService } from '../services/jobs/jobs.service';
import { State, process } from '@progress/kendo-data-query';
import { IJobReadModel, Job } from '../services/jobs/models';
import { listEnum } from '../services/common/functions';
import { ApiUrls } from '../services/common/api-urls';
import { SupplierModel } from '../shared/models/supplier.model';
import { SupplierService } from '../services/supplier.service';
import { PurchaseOrder, PurchaseOrderItem, PurchaseOrderStatus } from '../services/purchase-orders/models';
import { PurchaseOrderItemModalComponent } from './purchase-order-item-modal.component';
import { filter, tap } from 'rxjs';
import { DataStateChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';

@Component({
    selector: 'app-purchase-order-modal',
    templateUrl: 'purchase-order-modal.component.html'
})
export class PurchaseOrderModalComponent extends ModalComponent<PurchaseOrderModalOptions> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    @ViewChild('purchaseOrderItemModal', { static: true })
    purchaseOrderItemModal: PurchaseOrderItemModalComponent;

    jobs: SelectableJob[];
    job: Job;
    jobReadonly: boolean;
    status: PurchaseOrderStatus;
    selectedJob: SelectableJob;
    suppliers: SupplierModel[];
    gridState: State = {
        skip: 0,
        take: 30,
        sort: [
            { field: 'productName', dir: 'asc' }
        ]
    };
    gridData: GridDataResult;

    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;

    readonly states = listEnum<PurchaseOrderStatus>(PurchaseOrderStatus);

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
        private readonly _suppliersService: SupplierService
    ) {
        super();
    }

    ngOnInit() {
        this._getSuppliers();
        this._getJobs();
    }

    onSupplierChange() {
        this.options.purchaseOrder.supplierName = this.suppliers.find(e => e.id == this.options.purchaseOrder.supplierId)?.name ?? '';
    }

    addProduct() {
        const item = new PurchaseOrderItem(0, this.options.purchaseOrder.id, null, null, null, 1);

        this._subscriptions.push(
            this.purchaseOrderItemModal.open(item)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterProductAdded(item))
                )
                .subscribe()
        );
    }

    edit(item: PurchaseOrderItem) {
        const updatedItem = item.clone();

        this._subscriptions.push(
            this.purchaseOrderItemModal.open(updatedItem)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterProductUpdated(item, updatedItem))
                )
                .subscribe()
        );
    }

    askRemove(item: PurchaseOrderItem) {
        this._subscriptions.push(
            this._messageBox.confirm(`Sei sicuro di voler rimuovere il prodotto ${item.productName}?`)
                .pipe(
                    filter(e => e),
                    tap(() => this._afterProductRemoved(item))
                )
                .subscribe()
        );
    }

    onDataStateChange(state: DataStateChangeEvent | State) {
        this.gridState = state;
        this.console.log(state);
        this.gridData = process(this.options.purchaseOrder.items, this.gridState);
    }

    override open(options: PurchaseOrderModalOptions) {
        const result = super.open(options);

        if (this.options.purchaseOrder.jobId) {
            this._getJob(this.options.purchaseOrder.jobId);
        }

        this.jobReadonly = !!options.purchaseOrder.jobId;
        this.status = options.purchaseOrder.status;
        this.onDataStateChange(this.gridState);

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getJob(id: number) {
        this._subscriptions.push(
            this._jobsService.get(id)
                .pipe(
                    tap(e => this.job = e)
                )
                .subscribe()
        );
    }

    private _getSuppliers() {
        this._subscriptions.push(
            this._suppliersService.getSuppliersList()
                .pipe(
                    tap(e => this._setData(e))
                )
                .subscribe()
        );
    }

    private _setData(suppliers: SupplierModel[]) {
        this.suppliers = suppliers;
    }

    private _getJobs() {
        const state: State = {
            filter: {
                filters: [],
                logic: 'and'
            },
            sort: [
                { field: 'code', dir: 'desc' }
            ]
        };

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e)))
                )
                .subscribe()
        );
    }

    private _afterProductAdded(item: PurchaseOrderItem) {
        this.options.purchaseOrder.items.push(item);
        this.onDataStateChange(this.gridState);

        this._messageBox.success(`Prodotto aggiunto`);
    }

    private _afterProductUpdated(originalItem: PurchaseOrderItem, updatedItem: PurchaseOrderItem) {
        this.options.purchaseOrder.items.replace(originalItem, updatedItem);
        this.onDataStateChange(this.gridState);

        this._messageBox.success(`Prodotto aggiornato`);
    }

    private _afterProductRemoved(item: PurchaseOrderItem) {
        this.options.purchaseOrder.items.remove(item);
        this.onDataStateChange(this.gridState);

        this._messageBox.success(`Prodotto rimosso`);
    }

}

export class PurchaseOrderModalOptions {

    constructor(
        readonly purchaseOrder: PurchaseOrder
    ) {
    }

}

class SelectableJob {

    readonly id: number;
    readonly customer: string;
    readonly code: string;
    readonly fullName: string;
    readonly customerId: number;
    readonly addressId: number;
    readonly description: string;

    constructor(
        job: IJobReadModel
    ) {
        this.id = job.id;
        this.customer = job.customer;
        this.code = job.code;
        this.fullName = `${job.code} - ${job.customer}` + ((job.reference) ? ` - ${job.reference}` : ``);
        this.customerId = job.customerId;
        this.addressId = job.addressId;
        this.description = job.description;
    }

}
