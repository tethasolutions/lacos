import { Component, OnInit, ViewChild } from '@angular/core';
import { Activity } from '../services/activities/models';
import { ModalComponent } from '../shared/modal.component';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { tap } from 'rxjs';
import { ActivityTypesService } from '../services/activityTypes.service';
import { CustomerService } from '../services/customer.service';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { CustomerModel } from '../shared/models/customer.model';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { IJobReadModel } from '../services/jobs/models';

@Component({
    selector: 'app-activity-modal',
    templateUrl: 'activity-modal.component.html'
})
export class ActivityModalComponent extends ModalComponent<ActivityModalOptions> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    activityTypes: ActivityTypeModel[];
    customer: CustomerModel;
    jobs: SelectableJob[];
    jobReadonly: boolean;

    constructor(
        private readonly _activityTypesService: ActivityTypesService,
        private readonly _customersService: CustomerService,
        private readonly _messageBox: MessageBoxService,
        private readonly _jobsService: JobsService
    ) {
        super();
    }

    ngOnInit() {
        this._getActivityTypes();
    }

    onJobChanged() {
        this.options.activity.customerAddressId = null;

        this._tryGetCustomer();
    }

    override open(options: ActivityModalOptions) {
        const result = super.open(options);

        this.jobReadonly = !!options.activity.jobId;
        this.customer = null;
        this._getJobs();

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
                    tap(() => this._tryGetCustomer())
                )
                .subscribe()
        );
    }

    private _tryGetCustomer() {
        if (!this.options.activity.jobId) {
            this.customer = null;
            return;
        }

        const customerId = this.jobs
            .find(e => e.id === this.options.activity.jobId)
            .customerId;

        this._subscriptions.push(
            this._customersService.getCustomer(customerId)
                .pipe(
                    tap(e => this.customer = e)
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

    constructor(
        job: IJobReadModel
    ) {
        this.id = job.id;
        this.customer = job.customer;
        this.code = job.code;
        this.fullName = `${job.code} - ${job.customer}`;
        this.customerId = job.customerId;
    }

}
