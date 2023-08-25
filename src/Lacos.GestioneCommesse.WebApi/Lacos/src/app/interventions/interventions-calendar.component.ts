import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { IInterventionOperatorReadModel, IInterventionReadModel, Intervention, InterventionStatus } from '../services/interventions/models';
import { getToday } from '../services/common/functions';
import { DateChangeEvent, DateRange, EventClickEvent, SchedulerComponent, SlotClickEvent } from '@progress/kendo-angular-scheduler';
import { InterventionsService } from '../services/interventions/interventions.service';
import { State } from '@progress/kendo-data-query';
import { filter, switchMap, tap } from 'rxjs';
import { InterventionModalComponent } from './intervention-modal.component';
import { MessageBoxService } from '../services/common/message-box.service';

@Component({
    selector: 'app-interventions-calendar',
    templateUrl: 'interventions-calendar.component.html'
})
export class InterventionsCalendarComponent extends BaseComponent implements OnInit {

    @Input()
    interventionModal: InterventionModalComponent;

    @Input()
    options: InterventionsCalendarOptions;

    @Output()
    readonly interventionUpdated = new EventEmitter<Intervention>();

    @Output()
    readonly interventionCreated = new EventEmitter<Intervention>();

    @ViewChild('scheduler', { static: true })
    scheduler: SchedulerComponent;

    interventions: InterventionSchedulerModel[] = [];
    date: Date;

    private _dateRange: DateRange;

    constructor(
        private readonly _service: InterventionsService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() { }

    onDateChange(event: DateChangeEvent) {
        this._dateRange = event.dateRange;

        this._read();
    }

    onEventDblClick(event: EventClickEvent) {
        const intervention = event.event as InterventionSchedulerModel;

        this._edit(intervention.id);
    }

    onSlotDblClick(event: SlotClickEvent) {
        this._create(event.start);
    }

    private _read() {
        const state: State = {
            filter: {
                filters: [
                    {
                        filters: [
                            { field: 'start', operator: 'gte', value: this._dateRange.start },
                            { field: 'start', operator: 'lte', value: this._dateRange.end }
                        ],
                        logic: 'and'
                    },
                    {
                        filters: [
                            { field: 'end', operator: 'gte', value: this._dateRange.start },
                            { field: 'end', operator: 'lte', value: this._dateRange.end }
                        ],
                        logic: 'and'
                    },
                    {
                        filters: [
                            { field: 'start', operator: 'lte', value: this._dateRange.start },
                            { field: 'end', operator: 'gte', value: this._dateRange.end }
                        ],
                        logic: 'and'
                    }
                ],
                logic: 'or'
            }
        };

        this._subscriptions.push(
            this._service.read(state)
                .pipe(
                    tap(e => this.interventions = e.data.as<IInterventionReadModel>().map(ee => new InterventionSchedulerModel(ee)))
                )
                .subscribe()
        );
    }

    private _edit(id: number) {
        this._subscriptions.push(
            this._service.get(id)
                .pipe(
                    switchMap(e => this.interventionModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.interventionModal.options)),
                    tap(e => this._afterUpdated(e))
                )
                .subscribe()
        );
    }

    private _create(start: Date) {
        this.console.log(this.options);
        const intervention = new Intervention(0, start, start.addHours(1), InterventionStatus.Scheduled,
            null, null, this.options?.activityId, this.options?.jobId, [], []);

        this._subscriptions.push(
            this.interventionModal.open(intervention)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(intervention)),
                    tap(e => this._afterCreated(e))
                )
                .subscribe()
        );
    }

    private _afterUpdated(intervention: Intervention) {
        this._messageBox.success('Intervento aggiornato.');

        this.interventionUpdated.emit(intervention);

        this._read();
    }

    private _afterCreated(intervention: Intervention) {
        this._messageBox.success('Intervento programmato.');

        this.interventionCreated.emit(intervention);

        this._read();
    }

}

class InterventionSchedulerModel {

    readonly id: number;
    readonly status: InterventionStatus;
    readonly start: Date;
    readonly end: Date;
    readonly customer: string;
    readonly customerAddress: string;
    readonly description: string;
    readonly operators: IInterventionOperatorReadModel[];
    readonly activityType: string;

    constructor(
        intervention: IInterventionReadModel
    ) {
        this.id = intervention.id;
        this.status = intervention.status;
        this.start = new Date(intervention.start);
        this.end = new Date(intervention.end);
        this.customer = intervention.customer;
        this.customerAddress = intervention.customerAddress;
        this.description = intervention.description;
        this.operators = intervention.operators;
        this.activityType = intervention.activityType;
    }

}

export interface InterventionsCalendarOptions {

    readonly activityId: number,
    readonly jobId: number

}
