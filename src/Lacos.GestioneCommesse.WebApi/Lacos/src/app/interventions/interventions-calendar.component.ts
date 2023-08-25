import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { IInterventionOperatorReadModel, IInterventionReadModel, Intervention, InterventionStatus } from '../services/interventions/models';
import { DateChangeEvent, DateRange, DragEndEvent, EventClickEvent, RemoveEvent, ResizeEndEvent, SchedulerComponent, SlotClickEvent } from '@progress/kendo-angular-scheduler';
import { InterventionsService } from '../services/interventions/interventions.service';
import { State } from '@progress/kendo-data-query';
import { filter, switchMap, tap } from 'rxjs';
import { InterventionModalComponent } from './intervention-modal.component';
import { MessageBoxService } from '../services/common/message-box.service';

@Component({
    selector: 'app-interventions-calendar',
    templateUrl: 'interventions-calendar.component.html'
})
export class InterventionsCalendarComponent extends BaseComponent {

    @Input()
    interventionModal: InterventionModalComponent;

    @Input()
    options: InterventionsCalendarOptions;

    @Output()
    readonly interventionUpdated = new EventEmitter<Intervention>();

    @Output()
    readonly interventionCreated = new EventEmitter<Intervention>();

    @Output()
    readonly interventionRemoved = new EventEmitter<number>();

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

    onDateChange(event: DateChangeEvent) {
        this._dateRange = event.dateRange;

        this._read();
    }

    onEventDblClick(event: EventClickEvent) {
        const intervention = event.event as InterventionSchedulerModel;

        this._editIntervention(intervention.id);
    }

    onSlotDblClick(event: SlotClickEvent) {
        this._createIntervention(event.start);
    }

    onResizeEnd(event: ResizeEndEvent) {
        const intervention = event.dataItem as InterventionSchedulerModel;

        this._checkInterventionIsEditable(intervention);

        this._subscriptions.push(
            this._service.get(intervention.id)
                .pipe(
                    tap(e => this._resizeIntervention(e, event.start, event.end)),
                    switchMap(e => this._service.update(e)),
                    tap(e => this._afterInterventionUpdated(e))
                )
                .subscribe()
        );
    }

    onDragEnd(event: DragEndEvent) {
        const intervention = event.dataItem as InterventionSchedulerModel;

        this._checkInterventionIsEditable(intervention);

        this._subscriptions.push(
            this._service.get(intervention.id)
                .pipe(
                    tap(e => this._resizeIntervention(e, event.start, event.end)),
                    switchMap(e => this._service.update(e)),
                    tap(e => this._afterInterventionUpdated(e))
                )
                .subscribe()
        );
    }

    onRemove(event: RemoveEvent) {
        const intervention = event.dataItem as InterventionSchedulerModel;

        this._checkInterventionIsEditable(intervention);

        const text = `Sei sicuro di voler rimuovere l'intervento selezionato?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(intervention.id)),
                    tap(() => this._afterInterventionRemoved(intervention.id))
                )
                .subscribe()
        );
    }

    refresh() {
        this._read();
    }

    private _resizeIntervention(intervention: Intervention, start: Date, end: Date) {
        intervention.start = start;
        intervention.end = end;
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

    private _editIntervention(id: number) {
        this._subscriptions.push(
            this._service.get(id)
                .pipe(
                    switchMap(e => this.interventionModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.interventionModal.options)),
                    tap(e => this._afterInterventionUpdated(e))
                )
                .subscribe()
        );
    }

    private _createIntervention(start: Date) {
        const intervention = new Intervention(0, start, start.addHours(1), InterventionStatus.Scheduled,
            null, null, this.options?.activityId, this.options?.jobId, [], []);

        this._subscriptions.push(
            this.interventionModal.open(intervention)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(intervention)),
                    tap(e => this._afterInterventionCreated(e))
                )
                .subscribe()
        );
    }

    private _afterInterventionUpdated(intervention: Intervention) {
        this._messageBox.success('Intervento aggiornato.');

        this.interventionUpdated.emit(intervention);

        this._read();
    }

    private _afterInterventionCreated(intervention: Intervention) {
        this._messageBox.success('Intervento programmato.');

        this.interventionCreated.emit(intervention);

        this._read();
    }

    private _afterInterventionRemoved(id: number) {
        this._messageBox.success('Intervento rimosso.');

        this.interventionRemoved.emit(id);

        this._read();
    }

    private _checkInterventionIsEditable(intervention: InterventionSchedulerModel) {
        if (intervention.status === InterventionStatus.Scheduled) {
            return true;
        }

        const text = `Non puoi modificare o eliminare un intervento già completato.`;

        this._subscriptions.push(
            this._messageBox.alert(text, 'Attenzione')
                .subscribe()
        );

        return false;
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
