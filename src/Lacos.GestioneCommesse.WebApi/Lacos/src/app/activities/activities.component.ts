import { Component, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Activity, ActivityStatus, IActivityReadModel, activityStatusNames } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivatedRoute, Params } from '@angular/router';
import { ActivityModalComponent, ActivityModalOptions } from './activity-modal.component';
import { JobsService } from '../services/jobs/jobs.service';
import { of } from 'rxjs';

@Component({
    selector: 'app-activities',
    templateUrl: 'activities.component.html'
})
export class ActivitiesComponent extends BaseComponent implements OnInit {

    @ViewChild('activityModal', { static: true })
    activityModal: ActivityModalComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
                this._buildStatusFilter(),
                this._buildJobIdFilter(),
                this._buildTypeIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'expirationDate', dir: 'asc' }]
    };

    private _jobId: number;
    private _typeId: number;
    private cellArgs: CellClickEvent;

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _service: ActivitiesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _route: ActivatedRoute,
    ) {
        super();
    }

    ngOnInit() {
        this._subscribeRouteParams();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    askRemove(activity: IActivityReadModel) {
        const text = `Sei sicuro di voler rimuovere l'attività ${activity.number}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(activity.id)),
                    tap(() => this._afterRemoved(activity))
                )
                .subscribe()
        );
    }

    create() {
        const activity = new Activity(0, ActivityStatus.Pending, null, null, this._jobId, null, null, null, null, null, null, null, null, null);
        const options = new ActivityModalOptions(activity);

        this._subscriptions.push(
            this.activityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(activity)),
                    tap(() => this._afterActivityCreated())
                )
                .subscribe()
        );
    }

    edit(activity: IActivityReadModel) {
        this._subscriptions.push(
            this._service.get(activity.id)
                .pipe(
                    map(e => new ActivityModalOptions(e)),
                    switchMap(e => this.activityModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.activityModal.options.activity)),
                    tap(() => this._afterActivityUpdated())
                )
                .subscribe()
        );
    }

    onDblClick(): void {
        if (!this.cellArgs.isEdited) {
            this._subscriptions.push(
                this._service.get(this.cellArgs.dataItem.id)
                    .pipe(
                        map(e => new ActivityModalOptions(e)),
                        switchMap(e => this.activityModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._service.update(this.activityModal.options.activity)),
                        tap(() => this._afterActivityUpdated())
                    )
                    .subscribe()
            )
        }
    }

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    readonly rowCallback = (context: RowClassArgs) => {
        const activity = context.dataItem as IActivityReadModel;

        switch (true) {
            case activity.status === ActivityStatus.Completed:
                return { 'activity-completed': true };
            case activity.status === ActivityStatus.Pending:
                return { 'activity-pending': true };
            case activity.status === ActivityStatus.InProgress:
                return { 'activity-in-progress': true };
            case activity.status != ActivityStatus.Completed && !!activity.expirationDate && new Date(activity.expirationDate).addDays(1).isPast():
                return { 'activity-expired': true };
            default:
                return {};
        }
    };

    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    private _afterActivityCreated() {
        this._messageBox.success('Attività creata.')

        this._read();
    }

    private _afterActivityUpdated() {
        this._messageBox.success('Attività aggiornata.')

        this._read();
    }

    private _subscribeRouteParams() {
        this._route.queryParams
            .pipe(
                // switchMap(e =>
                //     +e['jobId']
                //         ? this._jobsService.get(+e['jobId'])
                //         : of(void 0)
                // ),                
                tap(e => this._setParams(e))
            )
            .subscribe();
    }

    private _afterRemoved(activity: IActivityReadModel) {
        const text = `Attività ${activity.number} rimossa.`;

        this._messageBox.success(text);

        this._read();
    }

    private _buildJobIdFilter() {
        const that = this;

        return {
            field: 'jobId',
            get operator() {
                return that._jobId
                    ? 'eq'
                    : 'isnotnull'
            },
            get value() {
                return that._jobId;
            }
        };
    }

    private _buildTypeIdFilter() {
        const that = this;

        return {
            field: 'typeId',
            get operator() {
                return that._typeId
                    ? 'eq'
                    : 'isnotnull'
            },
            get value() {
                return that._typeId;
            }
        };
    }

    private _buildStatusFilter() {
        const that = this;

        return {
            get field() {
                return that._jobId
                    ? 'id'
                    : undefined
            },
            get operator() {
                return that._jobId
                    ? 'isnotnull'
                    : undefined
            },
            get filters() {
                return that._jobId
                    ? undefined
                    : [ActivityStatus.Pending, ActivityStatus.InProgress]
                        .map(e => ({ field: 'status', operator: 'eq', value: e }))
            },
            logic: 'or'
        };
    }

    private _setParams(params: Params) {
        this._jobId = isNaN(+params['jobId']) ? null : +params['jobId'];
        this._typeId = isNaN(+params['typeId']) ? null : +params['typeId'];
        this._read();
    }

}
