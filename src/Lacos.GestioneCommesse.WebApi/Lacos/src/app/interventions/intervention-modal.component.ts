import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { Intervention, InterventionStatus } from '../services/interventions/models';
import { NgForm } from '@angular/forms';
import { tap } from 'rxjs';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { IJobReadModel } from '../services/jobs/models';
import { IActivityReadModel } from '../services/activities/models';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { ActivitiesService } from '../services/activities/activities.service';
import { VehicleModel } from '../shared/models/vehicle.model';
import { VehiclesService } from '../services/vehicles.service';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';

@Component({
    selector: 'app-intervention-modal',
    templateUrl: 'intervention-modal.component.html'
})
export class InterventionModalComponent extends ModalComponent<Intervention> implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    jobs: SelectableJob[];
    activities: SelectableActivity[];
    vehicles: SelectableVehicle[];
    activityReadonly: boolean;
    jobReadonly: boolean;
    readonly: boolean;
    operators: OperatorModel[];

    constructor(
        private readonly _jobsService: JobsService,
        private readonly _activitiesService: ActivitiesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _vechiclesService: VehiclesService,
        private readonly _operatorsService: OperatorsService
    ) {
        super();
    }

    ngOnInit() {
        this._getVehicles();
        this._getOperators();
    }

    onJobChanged() {
        this.options.activityId = null;
        this._tryGetActivities();
    }

    onOperatorsChanged() {
        const firstOperatorId = this.options.operators[0];

        if (firstOperatorId && !this.options.vehicleId) {
            this.options.vehicleId = this.operators
                .find(e => e.id === firstOperatorId)
                ?.defaultVehicleId;
        } else if (!firstOperatorId && this.options.vehicleId) {
            this.options.vehicleId = null;
        }
    }

    override open(options: Intervention) {
        const result = super.open(options);

        this.activityReadonly = !!this.options.activityId;
        this.jobReadonly = !!this.options.jobId;
        this.readonly = this.options.status !== InterventionStatus.Scheduled;

        this._getJobs();
        this._tryGetActivities();

        return result;
    }

    protected override _canClose() {
        markAsDirty(this.form);

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
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

        if (this.options.jobId) {
            state.filter.filters.push(
                { field: 'id', operator: 'eq', value: this.options.jobId }
            );
        }

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e)))
                )
                .subscribe()
        );
    }

    private _tryGetActivities() {
        if (!this.options.jobId) {
            this.activities = [];
            return;
        }

        const state: State = {
            filter: {
                filters: [
                    { field: 'jobId', operator: 'eq', value: this.options.jobId }
                ],
                logic: 'and'
            },
            sort: [
                { field: 'number', dir: 'asc' }
            ]
        };

        if (this.options.activityId) {
            state.filter.filters.push(
                { field: 'id', operator: 'eq', value: this.options.activityId }
            );
        }

        this._subscriptions.push(
            this._activitiesService.read(state)
                .pipe(
                    tap(e => this.activities = (e.data as IActivityReadModel[]).map(e => new SelectableActivity(e)))
                )
                .subscribe()
        );
    }

    private _getVehicles() {
        this._subscriptions.push(
            this._vechiclesService.readVehiclesList()
                .pipe(
                    tap(e => this.vehicles = e.map(ee => new SelectableVehicle(ee)))
                )
                .subscribe()
        )
    }

    private _getOperators() {
        const state: State = {
            sort: [
                { field: 'name', dir: 'desc' }
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

}

class SelectableJob {

    readonly id: number;
    readonly customer: string;
    readonly code: string;
    readonly fullName: string;

    constructor(
        job: IJobReadModel
    ) {
        this.id = job.id;
        this.customer = job.customer;
        this.code = job.code;
        this.fullName = `${job.code} - ${job.customer}`;
    }

}

class SelectableActivity {

    readonly id: number;
    readonly customerAddress: string;
    readonly number: number;
    readonly fullName: string;

    constructor(
        activity: IActivityReadModel
    ) {
        this.id = activity.id;
        this.customerAddress = activity.customerAddress;
        this.number = activity.number;
        this.fullName = `${activity.number} - ${activity.customerAddress}`;
    }

}

class SelectableVehicle {

    readonly id: number;
    readonly name: string;
    readonly plate: string;
    readonly fullName: string;

    constructor(
        vehicle: VehicleModel
    ) {
        this.id = vehicle.id;
        this.name = vehicle.name;
        this.plate = vehicle.plate;
        this.fullName = `${vehicle.plate} - ${vehicle.name}`;
    }

}
