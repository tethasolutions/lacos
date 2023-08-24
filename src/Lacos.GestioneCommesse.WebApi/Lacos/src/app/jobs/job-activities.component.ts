import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, switchMap, tap } from 'rxjs/operators';
import { ActivityStatus, IActivityReadModel, activityStatusNames } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { IJobReadModel } from '../services/jobs/models';

@Component({
    selector: 'app-job-activities',
    templateUrl: 'job-activities.component.html'
})
export class JobActivitiesComponent extends BaseComponent implements OnChanges {

    @Input()
    job: IJobReadModel;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 10,
        filter: {
            filters: [
                {
                    filters: [ActivityStatus.Pending, ActivityStatus.InProgress]
                        .map(e => ({ field: 'status', operator: 'eq', value: e })),
                    logic: 'or'
                },
                this._buildJobIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'number', dir: 'desc' }]
    };

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _service: ActivitiesService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['job'] && this.job) {
            this._read();
        }
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
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

    private _afterRemoved(activity: IActivityReadModel) {
        const text = `Attività ${activity.number} rimossa.`;

        this._messageBox.success(text);

        this._read();
    }

    private _buildJobIdFilter() {
        const that = this;

        return {
            field: 'jobId',
            operator: 'eq',
            get value() { return that.job?.id; }
        };
    }

}
