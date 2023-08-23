import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { JobsService } from '../services/jobs/jobs.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, switchMap, tap } from 'rxjs/operators';
import { JobModalComponent } from '../job-modal/job-modal.component';
import { IJobReadModel, Job, JobStatus, jobStates } from '../services/jobs/models';
import { getToday } from '../services/common/functions';

@Component({
    selector: 'app-jobs',
    templateUrl: 'jobs.component.html'
})
export class JobsComponent extends BaseComponent implements OnInit {

    @ViewChild('jobModal', { static: true })
    jobModal: JobModalComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 15,
        filter: {
            filters: [
                {
                    filters: [JobStatus.Pending, JobStatus.InProgress]
                        .map(e => ({ field: 'status', operator: 'eq', value: e })),
                    logic: 'or'
                }
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'date', dir: 'asc' }]
    };

    readonly jobStates = jobStates;

    constructor(
        private readonly _service: JobsService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._readJobs();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._readJobs();
    }

    protected _readJobs() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    create() {
        const today = getToday();
        const job = new Job(0, null, today.getFullYear(), today, null, JobStatus.Pending, null);

        this._subscriptions.push(
            this.jobModal.open(job)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(job)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    edit(job: IJobReadModel) {
        this._subscriptions.push(
            this._service.get(job.id)
                .pipe(
                    switchMap(e => this.jobModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.jobModal.options)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    askRemove(job: IJobReadModel) {
        const text = `Sei sicuro di voler rimuovere la commessa ${job.code}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(job.id)),
                    tap(() => this._afterRemoved(job))
                )
                .subscribe()
        );
    }

    private _afterSaved(job: Job) {
        this._messageBox.success(`Commessa ${job.code} salvata.`);

        this._readJobs();
    }

    private _afterRemoved(job: IJobReadModel) {
        const text = `Commessa ${job.code} rimossa.`;

        this._messageBox.success(text);

        this._readJobs();
    }

}
