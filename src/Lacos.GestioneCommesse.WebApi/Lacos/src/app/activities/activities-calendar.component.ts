import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { Activity, ActivityStatus, IActivityReadModel } from '../services/activities/models';
import { DateChangeEvent, DateRange, DragEndEvent, EventClickEvent, EventStyleArgs, RemoveEvent, ResizeEndEvent, SchedulerComponent, SlotClickEvent } from '@progress/kendo-angular-scheduler';
import { ActivitiesService } from '../services/activities/activities.service';
import { State } from '@progress/kendo-data-query';
import { filter, switchMap, tap } from 'rxjs';
import { ActivityModalComponent, ActivityModalOptions } from './activity-modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { ActivityTypesService } from '../services/activityTypes.service';
import { ActivityTypeModel } from '../shared/models/activity-type.model';

@Component({
    selector: 'app-activities-calendar',
    templateUrl: 'activities-calendar.component.html'
})
export class ActivitiesCalendarComponent extends BaseComponent implements OnInit {

    @ViewChild('activityModal', { static: true })
    activityModal: ActivityModalComponent;

    @ViewChild('scheduler', { static: true })
    scheduler: SchedulerComponent;

    activities: ActivitySchedulerModel[] = [];
    date: Date;
    activityTypes: SelectableActivityType[];

    private _dateRange: DateRange;

    constructor(
        private readonly _service: ActivitiesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _activityTypesService: ActivityTypesService
    ) {
        super();
    }

    ngOnInit() {
        this._getActivityTypes();
    }

    onDateChange(event: DateChangeEvent) {
        this._dateRange = event.dateRange;

        if (this.activityTypes) {
            this._read();
        }
    }

    onEventDblClick(event: EventClickEvent) {
        const activity = event.event as ActivitySchedulerModel;

        this._editActivity(activity.id);
    }

    onSlotDblClick(event: SlotClickEvent) {
        this._createActivity(event.start);
    }

    onResizeEnd(event: ResizeEndEvent) {
        const activity = event.dataItem as ActivitySchedulerModel;

        if (!activity.canBeEdited) {
            this._showAlert(`Non puoi modificare un'attività già completata.`);
            return;
        }

        this._subscriptions.push(
            this._service.get(activity.id)
                .pipe(
                    tap(e => this._resizeActivity(e, event.start, event.end, false)),
                    switchMap(e => this._service.update(e)),
                    tap(e => this._afterActivityUpdated(e))
                )
                .subscribe()
        );
    }

    onDragEnd(event: DragEndEvent) {
        const activity = event.dataItem as ActivitySchedulerModel;

        if (!activity.canBeEdited) {
            this._showAlert(`Non puoi modificare un'attività già completata.`);
            return;
        }

        this._subscriptions.push(
            this._service.get(activity.id)
                .pipe(
                    tap(e => this._resizeActivity(e, event.start, event.end, event.isAllDay)),
                    switchMap(e => this._service.update(e)),
                    tap(e => this._afterActivityUpdated(e))
                )
                .subscribe()
        );
    }

    onRemove(event: RemoveEvent) {
        const activity = event.dataItem as ActivitySchedulerModel;

        if (!activity.canBeRemoved) {
            this._showAlert(`Non puoi eliminare un'attività già completata.`);
            return;
        }

        const text = `Sei sicuro di voler rimuovere l'attività selezionata?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(activity.id)),
                    tap(() => this._afterActivityRemoved(activity.id))
                )
                .subscribe()
        );
    }

    refresh() {
        this._read();
    }

    toggleActivityType(activityType: SelectableActivityType) {
        activityType.selected = !activityType.selected;

        const allUnselected = this.activityTypes
            .all(e => !e.selected);

        if (allUnselected) {
            this.activityTypes
                .forEach(e => e.selected = true);
        }

        this._read();
    }

    eventStylesCallback = (args: EventStyleArgs) => {
        const activity = args.event.dataItem as ActivitySchedulerModel;
        return { 'background-color': activity.activityColor };
    }

    private _resizeActivity(activity: Activity, start: Date, end: Date, isAllDay: boolean) {
        if (isAllDay) {
            start = start.toDateWithoutTime();
            end = start.addDays(1);
        }

        activity.startDate = start;
        activity.expirationDate = end;
    }

    private _read() {
        const state: State = {
            filter: {
                filters: [
                    {
                        filters: [
                            {
                                filters: [
                                    { field: 'startDate', operator: 'gte', value: this._dateRange.start },
                                    { field: 'startDate', operator: 'lte', value: this._dateRange.end }
                                ],
                                logic: 'and'
                            },
                            {
                                filters: [
                                    { field: 'expirationDate', operator: 'gte', value: this._dateRange.start },
                                    { field: 'expirationDate', operator: 'lte', value: this._dateRange.end }
                                ],
                                logic: 'and'
                            },
                            {
                                filters: [
                                    { field: 'startDate', operator: 'lte', value: this._dateRange.start },
                                    { field: 'expirationDate', operator: 'gte', value: this._dateRange.end }
                                ],
                                logic: 'and'
                            }
                        ],
                        logic: 'or'
                    },
                    {
                        filters: [
                            { field: 'startDate', operator: 'isnotnull', value: null },
                            { field: 'expirationDate', operator: 'isnotnull', value: null }
                        ],
                        logic: 'and'
                    }
                ],
                logic: 'and'
            }
        };

        state.filter.filters.push(
            {
                filters: this.activityTypes
                    .filter(e => e.selected)
                    .map(e => ({ field: 'typeId', operator: 'eq', value: e.id })),
                logic: 'or'
            }
        );

        this._subscriptions.push(
            this._service.read(state)
                .pipe(
                    tap(e => this.activities = (e.data as IActivityReadModel[])
                        .filter(ee => ee.startDate && ee.expirationDate)
                        .map(ee => new ActivitySchedulerModel(ee)))
                )
                .subscribe()
        );
    }

    private _editActivity(id: number) {
        this._subscriptions.push(
            this._service.get(id)
                .pipe(
                    switchMap(e => this.activityModal.open(new ActivityModalOptions(e))),
                    filter(e => e),
                    switchMap(() => this._service.update(this.activityModal.options.activity)),
                    tap(e => this._afterActivityUpdated(e))
                )
                .subscribe()
        );
    }

    private _createActivity(start: Date) {
        const startDate = new Date(start);
        startDate.setHours(8, 0, 0, 0);
        const expirationDate = new Date(start);
        expirationDate.setHours(18, 0, 0, 0);

        const activity = new Activity(0, ActivityStatus.Pending, 0, '', null, null,
            null, null, null, null, null, startDate, expirationDate,
            false, null, null, null, null, false, false, false, false, [], []);

        this._subscriptions.push(
            this.activityModal.open(new ActivityModalOptions(activity))
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(this.activityModal.options.activity)),
                    tap(e => this._afterActivityCreated(e))
                )
                .subscribe()
        );
    }

    private _afterActivityUpdated(_activity: Activity) {
        this._messageBox.success('Attività aggiornata.');

        this._read();
    }

    private _afterActivityCreated(_activity: Activity) {
        this._messageBox.success('Attività programmata.');

        this._read();
    }

    private _afterActivityRemoved(_id: number) {
        this._messageBox.success('Attività rimossa.');

        this._read();
    }

    private _showAlert(text: string) {
        this._subscriptions.push(
            this._messageBox.alert(text, 'Attenzione')
                .subscribe()
        );
    }

    private _getActivityTypes() {
        this._subscriptions.push(
            this._activityTypesService.readActivityTypesActivityCalendar()
                .pipe(
                    tap(e => this._setActivityTypes(e))
                )
                .subscribe()
        );
    }

    private _setActivityTypes(activityTypes: ActivityTypeModel[]) {
        this.activityTypes = activityTypes
            .map(e => new SelectableActivityType(e.id, e.colorHex, e.name));

        if (this._dateRange) {
            this._read();
        }
    }

}

class ActivitySchedulerModel {

    readonly id: number;
    readonly status: ActivityStatus;
    readonly start: Date;
    readonly end: Date;
    readonly customer: string;
    readonly address: string;
    readonly type: string;
    readonly activityColor: string;
    readonly isAllDay: boolean;
    readonly canBeRemoved: boolean;
    readonly canBeEdited: boolean;
    readonly referentId: number;
    readonly referentName: string;
    readonly jobCode: string;

    constructor(
        activity: IActivityReadModel
    ) {
        this.id = activity.id;
        this.status = activity.status;
        this.start = new Date(activity.startDate);
        const end = new Date(activity.expirationDate);
        const endIsMidnight = end.getHours() === 0 && end.getMinutes() === 0 && end.getSeconds() === 0;
        this.end = endIsMidnight ? end.addDays(1) : end;
        this.customer = activity.customer;
        this.address = activity.address;
        this.type = activity.type;
        this.activityColor = activity.activityColor;
        this.isAllDay = this.start.addDays(1).hasSameDateAndTime(this.end);
        this.canBeRemoved = activity.canBeRemoved;
        this.canBeEdited = activity.status !== ActivityStatus.Completed;
        this.referentId = activity.referentId;
        this.referentName = activity.referentName;
        this.jobCode = activity.jobCode;
    }

}

class SelectableActivityType {

    selected = true;

    constructor(
        readonly id: number,
        readonly colorHex: string,
        readonly name: string
    ) {
    }

}
