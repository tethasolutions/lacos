import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { IJobReadModel, Job } from '../services/jobs/models';
import { listEnum } from '../services/common/functions';
import { ApiUrls } from '../services/common/api-urls';
import { SupplierModel } from '../shared/models/supplier.model';
import { SupplierService } from '../services/supplier.service';
import { PurchaseOrder, PurchaseOrderItem, PurchaseOrderStatus } from '../services/purchase-orders/models';
import { CustomerModel } from '../shared/models/customer.model';
import { PurchaseOrderItemModalComponent } from './purchase-order-item-modal.component';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { filter, map, switchMap, tap } from 'rxjs';

@Component({
    selector: 'app-purchase-order-modal',
    templateUrl: 'purchase-order-modal.component.html'
})
export class PurchaseOrderModalComponent extends ModalComponent<PurchaseOrderModalOptions> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('purchaseOrderItemModal', { static: true }) purchaseOrderItemModal: PurchaseOrderItemModalComponent;

    jobs: SelectableJob[];
    job: Job;
    jobReadonly: boolean;
    status: PurchaseOrderStatus;
    selectedJob: SelectableJob;
    suppliers: SupplierModel[];
    gridState: State = {
        skip: 0,
        take: 30,
        sort: [{ field: 'productName', dir: 'asc' }]
    };

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/purchase-order`;
    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;

    readonly states = listEnum<PurchaseOrderStatus>(PurchaseOrderStatus);

    constructor(
        private readonly _service: PurchaseOrdersService,
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

    override open(options: PurchaseOrderModalOptions) {
        const result = super.open(options);

        if (this.options.purchaseOrder.jobId) {
            this._subscriptions.push(
                this._jobsService.get(this.options.purchaseOrder.jobId)
                    .pipe(
                        tap(e => {
                            this.job = e;
                        })
                    )
                    .subscribe()
            );
        }

        this.jobReadonly = !!options.purchaseOrder.jobId;
        this.status = options.purchaseOrder.status;

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
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

    protected addProduct() {
        const request = new PurchaseOrderItem(0, this.options.purchaseOrder.id, null, null, null, 1);
        //if (this.options.purchaseOrder.id == null) {
            this.purchaseOrderItemModal.open(request)
                .pipe(
                    filter(e => e),
                    tap(e => {
                        this.options.purchaseOrder.items.push(request);
                        this._messageBox.success(`Prodotto aggiunto`);
                    })
                )
                .subscribe()
        //}
        //   else {
        //     request.checkListId = this.options.id;
        //     this._subscriptions.push(
        //         this.checklistItemModal.open(request)
        //             .pipe(
        //                 filter(e => e),
        //                 switchMap(() => this._checkListService.createCheckListItem(request)),
        //                 tap(e => {
        //                   this._messageBox.success(`Voce checklist creata`);
        //                 }),
        //                 tap(() => this._readCheckListItems())
        //             )
        //             .subscribe()
        //     );
        //   }
    }

    edit(item: PurchaseOrderItem) {
        this._subscriptions.push(
            this._service.getItem(item.id)
                .pipe(
                    map(e => this.purchaseOrderItemModal.open(e)),
                    switchMap(() => this._service.updateItem(this.purchaseOrderItemModal.options)),
                    //tap(() => this._afterPurchaseOrderUpdated())
                )
                .subscribe()
        );
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
