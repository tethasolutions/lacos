import { Component, OnInit, ViewChild } from '@angular/core';
import { Activity, ActivityStatus } from '../services/activities/models';
import { ModalComponent } from '../shared/modal.component';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { filter, map, switchMap, tap } from 'rxjs';
import { ActivityTypesService } from '../services/activityTypes.service';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModel } from '../shared/models/customer.model';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { IJobReadModel, Job } from '../services/jobs/models';
import { listEnum } from '../services/common/functions';
import { AddressModel } from '../shared/models/address.model';
import { AddressesService } from '../services/addresses.service';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { ActivityAttachmentUploadFileModel } from '../services/activities/activity-attachment-upload-file.model';
import { FileInfo, SuccessEvent } from '@progress/kendo-angular-upload';
import { SupplierModel } from '../shared/models/supplier.model';
import { SupplierService } from '../services/supplier.service';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { ActivityAttachmentModel } from '../services/activities/activity-attachment.model';
import { SupplierModalComponent } from '../supplier-modal/supplier-modal.component';

@Component({
    selector: 'app-activity-modal',
    templateUrl: 'activity-modal.component.html'
})
export class ActivityModalComponent extends ModalComponent<ActivityModalOptions> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;
    @ViewChild('supplierModal', { static: true }) supplierModal: SupplierModalComponent;

    activityTypes: ActivityTypeModel[];
    customer: CustomerModel;
    jobs: SelectableJob[];
    job: Job;
    jobReadonly: boolean;
    status: ActivityStatus;
    selectedActivityType: ActivityTypeModel;
    selectedJob: SelectableJob;
    suppliers: SupplierModel[];
    addresses: AddressModel[];
    operators: OperatorModel[];

    attachments: Array<FileInfo> = [];

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;

    pathImage = `${ApiUrls.baseUrl}/attachments/`;
    uploadSaveUrl = `${this._baseUrl}/activity-attachment/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/activity-attachment/remove-file`;

    readonly states = listEnum<ActivityStatus>(ActivityStatus);

    constructor(
        private readonly _activityTypesService: ActivityTypesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
        private readonly _suppliersService: SupplierService,
        private readonly _addressesService: AddressesService,
        private readonly _operatorsService: OperatorsService
    ) {
        super();
    }

    ngOnInit() {
        this._getActivityTypes();
        this._getSuppliers();
        this._getOperators();
    }

    onJobChanged() {
        this.options.activity.addressId = null;

        this._tryGetAddress();
    }

    onActivityTypeChange() {
        this.selectedActivityType = this.activityTypes.find(e => e.id == this.options.activity.typeId);
        this.selectedJob = this.jobs.find(e => e.id == this.options.activity.jobId);
        if (this.selectedActivityType.isInternal) {
            this.onSupplierChange();
            if (!this.options.activity.supplierId) {
                this.options.activity.addressId = null;
                const customerId = this.jobs
                    .find(e => e.id === this.options.activity.jobId).customerId;
                this.readAddresses(customerId);
            }
            //this.options.activity.description = this.selectedJob.description;
        }
        else {
            const customerId = this.jobs
                .find(e => e.id === this.options.activity.jobId).customerId;
            this.readAddresses(customerId);
        }
    }

    createSupplier() {
        const request = new SupplierModel();

        this._subscriptions.push(
            this.supplierModal.open(request)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._suppliersService.createSupplier(request)),
                    tap(e => {
                        this.options.activity.supplierId = e.id;
                        this._messageBox.success(`Fornitore ${request.name} creato`);
                    }),
                    tap(() => this._getSuppliers())
                )
                .subscribe()
        );
    }

    onSupplierChange() {
        const selectedSupplier = this.suppliers.find(e => e.id == this.options.activity.supplierId);
        this.addresses = selectedSupplier?.addresses ?? [];
        if (selectedSupplier != undefined) {
            const selectedAddress: AddressModel = selectedSupplier.addresses.find(x => x.isMainAddress == true);
            if (selectedAddress != undefined) {
                this.options.activity.addressId = selectedAddress.id;
            }
        }
    }

    override open(options: ActivityModalOptions) {
        const result = super.open(options);

        this.attachments = [];
        if (options.activity.attachments != null) {
            this.options.activity.attachments.forEach(element => {
                if (element.displayName != null && element.fileName != null) {
                    this.attachments.push({ name: element.displayName });
                }
            });
        }
        if (this.options.activity.jobId) {
            this._subscriptions.push(
                this._jobsService.get(this.options.activity.jobId)
                    .pipe(
                        tap(e => {
                            this.job = e;
                        })
                    )
                    .subscribe()
            );
        }

        this.jobReadonly = !!options.activity.jobId;
        this.customer = null;
        this.status = options.activity.status;
        this._getJobs();

        if (this.options.activity.typeId) {
            this.selectedActivityType = this.activityTypes.find(e => e.id == this.options.activity.typeId);
            if (this.selectedActivityType.isInternal && this.options.activity.supplierId) {
                this.onSupplierChange();
            }
        }

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getActivityTypes() {
        this._subscriptions.push(
            this._activityTypesService.readActivityTypesList()
                .pipe(
                    tap(e => this.activityTypes = e)
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
                        if (this.selectedActivityType) {
                            if (this.selectedActivityType.isInternal && this.options.activity.supplierId) {
                                this.onSupplierChange();
                            }
                        }
                    })

                )
                .subscribe()
        );
    }

    private _getOperators() {
        const state: State = {
            sort: [
                { field: 'name', dir: 'asc' }
            ]
        };

        this._subscriptions.push(
            this._operatorsService.readOperators(state)
                .pipe(
                    tap(e => this.operators = e.data as OperatorModel[])
                )
                .subscribe()
        )
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

        if (this.options.activity.jobId) {
            state.filter.filters.push(
                { field: 'id', operator: 'eq', value: this.options.activity.jobId }
            );
        }

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e))),
                    tap(() => this._tryGetAddress())
                )
                .subscribe()
        );
    }

    private _tryGetAddress() {
        if (!this.options.activity.jobId) {
            this.addresses = [];
            this.options.activity.addressId = null;
            return;
        }

        const customerId = this.jobs
            .find(e => e.id === this.options.activity.jobId).customerId;

        this.readAddresses(customerId);

        if (this.options.activity.addressId) return;

        const addressId = this.jobs
            .find(e => e.id === this.options.activity.jobId).addressId;
        this.options.activity.addressId = addressId;
    }

    createAddress() {
        const request = new AddressModel();
        this._subscriptions.push(
            this.addressModal.open(request)
                .pipe(
                    filter(e => e),
                    tap(() => {
                        this.addNewAddress(request);
                    })
                )
                .subscribe()
        );
    }

    addNewAddress(address: AddressModel) {
        if (this.job.customerId !== null) address.customerId = this.job.customerId;
        this._subscriptions.push(
            this._addressesService.createAddress(address)
                .pipe(
                    map(e => e),
                    tap(e => {
                        this.readAddresses(this.job.customerId);
                        this.options.activity.addressId = e.id;
                        this._messageBox.success(`Indirizzo creato con successo`);
                    })
                )
                .subscribe()
        );
    }

    readAddresses(customerId: number) {
        this._subscriptions.push(
            this._addressesService.getCustomerAddresses(customerId)
                .pipe(
                    map(e => {
                        this.addresses = e;
                    }),
                    tap(() => { })
                )
                .subscribe()
        );
    }

    downloadAttachment(fileName: string) {
        const attachment = this.options.activity.attachments
            .find(e => e.displayName === fileName);
        const url = `${this._baseUrl}/activity-attachment/download-file/${attachment.fileName}/${attachment.displayName}`;

        window.open(url);
    }

    public AttachmentExecutionSuccess(e: SuccessEvent): void {
        const file = e.response.body as ActivityAttachmentUploadFileModel;
        if (file != null) {
            let activityAttachmentModal = new ActivityAttachmentModel(0, file.originalFileName, file.fileName, this.options.activity.id);
            this.options.activity.attachments.push(activityAttachmentModal);
        } else {
            const deletedFile = e.files[0].name;
            this.options.activity.attachments.findAndRemove(e => e.displayName === deletedFile);
        }
    }
}

export class ActivityModalOptions {

    constructor(
        readonly activity: Activity
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
