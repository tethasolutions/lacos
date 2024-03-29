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
import { filter, switchMap, tap } from 'rxjs';
import { DataStateChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { FileInfo, SuccessEvent } from '@progress/kendo-angular-upload';
import { PurchaseOrderAttachmentUploadFileModel } from '../services/purchase-orders/purchage-order-attachment-upload-file.model';
import { PurchaseOrderAttachmentModel } from '../services/purchase-orders/purchase-order-attachment.model';
import { SupplierModalComponent } from '../supplier-modal/supplier-modal.component';

@Component({
    selector: 'app-purchase-order-modal',
    templateUrl: 'purchase-order-modal.component.html'
})
export class PurchaseOrderModalComponent extends ModalComponent<PurchaseOrderModalOptions> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    @ViewChild('purchaseOrderItemModal', { static: true }) purchaseOrderItemModal: PurchaseOrderItemModalComponent;
    @ViewChild('supplierModal', { static: true }) supplierModal: SupplierModalComponent;

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
    
    attachments: Array<FileInfo> = [];

    readonly imagesUrl = `${ApiUrls.baseAttachmentsUrl}/`;
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/purchase-orders`;
    uploadSaveUrl = `${this._baseUrl}/purchase-order-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/purchase-order-attachment/remove-file`;

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

        this.attachments = [];
        if (options.purchaseOrder.attachments != null) {
            this.options.purchaseOrder.attachments.forEach(element => {
                if (element.displayName != null && element.fileName != null) {
                    this.attachments.push({ name: element.displayName });
                }
            });
        }

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

    createSupplier() {
        const request = new SupplierModel();

        this._subscriptions.push(
            this.supplierModal.open(request)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._suppliersService.createSupplier(request)),
                    tap(e => {
                        this.options.purchaseOrder.supplierId = e.id;
                        this._messageBox.success(`Fornitore ${request.name} creato`);
                    }),
                    tap(() => this._getSuppliers())
                )
                .subscribe()
        );
    }

    private _getSuppliers() {
        this._subscriptions.push(
            this._suppliersService.getSuppliersList()
                .pipe(
                    tap(e => this._setData(e)),
                    tap(() => {
                        if (this.options.purchaseOrder.supplierId) {
                            this.onSupplierChange();
                        }})
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
                { field: 'date', dir: 'asc' }
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

    downloadAttachment(fileName: string) {
        const attachment = this.options.purchaseOrder.attachments
            .find(e => e.displayName === fileName);
        const url = `${this._baseUrl}/purchase-order-attachment/download-file/${attachment.fileName}/${attachment.displayName}`;

        window.open(url);
    }

    public AttachmentExecutionSuccess(e: SuccessEvent): void {
        const file = e.response.body as PurchaseOrderAttachmentUploadFileModel;
        if (file != null) {
            let purchaseOrderAttachmentModal = new PurchaseOrderAttachmentModel(0, file.originalFileName, file.fileName, this.options.purchaseOrder.id);
            this.options.purchaseOrder.attachments.push(purchaseOrderAttachmentModal);
        } else {
            const deletedFile = e.files[0].name;
            this.options.purchaseOrder.attachments.findAndRemove(e => e.displayName === deletedFile);
        }
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
