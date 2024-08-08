import { Component, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridComponent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { JobsService } from '../services/jobs/jobs.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { JobModalComponent } from './job-modal.component';
import { IJobProgressStatus, IJobReadModel, Job, JobCopy, JobStatus, jobStatusNames } from '../services/jobs/models';
import { Router } from '@angular/router';
import { StorageService } from '../services/common/storage.service';
import { UserService } from '../services/security/user.service';
import { User } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';

@Component({
    selector: 'app-jobs-progress-status',
    templateUrl: 'jobs-progress-status.component.html'
})
export class JobsProgressStatusComponent extends BaseComponent implements OnInit {

    @ViewChild('jobModal', { static: true })
    jobModal: JobModalComponent;

    @ViewChild('grid', { static: true })
    grid: GridComponent;
    user: User;
    currentOperator: OperatorModel;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'jobDate', dir: 'desc' },{ field: 'jobCode', dir: 'desc' }]
    };

    readonly jobStatusNames = jobStatusNames;

    constructor(
        private readonly _service: JobsService,
        private router: Router,
        private readonly _storageService: StorageService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this._read();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._saveState();
        this._read();
    }

    private _resumeState() {
        const savedState = this._storageService.get<State>(window.location.hash, true);
        if (savedState == null) return;
        this.gridState = savedState;
    }

    private _saveState() {
        this._storageService.save(this.gridState, window.location.hash, true);
    }

    protected _read() {
        this._subscriptions.push(
            this._service.getJobsProgressStatus(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    readonly rowCallback = (context: RowClassArgs) => {
        const job = context.dataItem as IJobProgressStatus;

        switch (true) {
            case job.jobStatus === JobStatus.Completed:
                return { 'job-completed': true };
            case job.jobStatus === JobStatus.InProgress:
                return { 'job-inprogress': true };
            case job.jobStatus === JobStatus.Pending:
                return { 'job-pending': true };
            case job.jobStatus === JobStatus.Billing:
                return { 'job-billing': true };
            case job.jobStatus === JobStatus.Billed:
                return { 'job-billed': true };
            default:
                return {};
        }
    };

}
