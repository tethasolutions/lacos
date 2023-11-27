import { Component, OnInit, ViewChild } from '@angular/core';
import { Activity, ActivityStatus } from '../services/activities/models';
import { ModalComponent } from '../shared/modal.component';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { filter, map, tap } from 'rxjs';
import { ActivityTypesService } from '../services/activityTypes.service';
import { CustomerService } from '../services/customer.service';
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

@Component({
    selector: 'app-activity-modal',
    templateUrl: 'activity-modal.component.html'
})
export class ActivityModalComponent extends ModalComponent<ActivityModalOptions> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;
    @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;

    activityTypes: ActivityTypeModel[];
    customer: CustomerModel;
    jobs: SelectableJob[];
    job: Job;
    jobReadonly: boolean;
    status: ActivityStatus;
    selectedActivityType: ActivityTypeModel;
    selectedJob: SelectableJob;
    addresses: AddressModel[];

    readonly states = listEnum<ActivityStatus>(ActivityStatus);

    constructor(
        private readonly _activityTypesService: ActivityTypesService,
        private readonly _customersService: CustomerService,
        private readonly _messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
        private readonly _addressesService: AddressesService
    ) {
        super();
    }

    ngOnInit() {
        this._getActivityTypes();
    }

    onJobChanged() {
        this.options.activity.addressId = null;

        this._tryGetAddress();
    }

    onActivityTypeChange() {
        this.selectedActivityType = this.activityTypes.find(e => e.id == this.options.activity.typeId);
        this.selectedJob = this.jobs.find(e => e.id == this.options.activity.jobId);
        if (this.selectedActivityType.isInternal) {
            if (!this.options.activity.id) this.options.activity.addressId = null;
            this.options.activity.description = this.selectedJob.description;
        }
    }

    override open(options: ActivityModalOptions) {
        const result = super.open(options);

        if (this.options.activity.jobId){
        this._subscriptions.push(
            this._jobsService.get(this.options.activity.jobId)
                .pipe(
                    tap(e => {
                        this.job = e;
                        this.readAddresses(this.job.customerId)
                    })
                )
                .subscribe()
        );
    }

        this.jobReadonly = !!options.activity.jobId;
        this.customer = null;
        this.status = options.activity.status;
        this._getJobs();

        this.selectedActivityType = this.activityTypes.find(e => e.id == this.options.activity.typeId);

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

        if (this.options.activity.addressId) return;

        const addressId = this.jobs
            .find(e => e.id === this.options.activity.jobId).addressId;

        const customerId = this.jobs
            .find(e => e.id === this.options.activity.jobId).customerId;

        this.readAddresses(customerId);
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
