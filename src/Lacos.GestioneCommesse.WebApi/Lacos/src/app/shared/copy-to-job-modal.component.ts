import { Component, OnInit } from '@angular/core';
import { ModalFormComponent } from './modal.component';
import { tap } from 'rxjs';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { IJobReadModel, JobStatus } from '../services/jobs/models';
import { CopyToJobModel } from './models/copy-to-job.model';

@Component({
    selector: 'app-copy-to-job-modal',
    templateUrl: 'copy-to-job-modal.component.html'
})
export class CopyToJobModalComponent extends ModalFormComponent<CopyToJobModel> implements OnInit {

    jobs: SelectableJob[];

    constructor(
        messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
    ) {
        super(messageBox);
    }

    ngOnInit() {
    }

    override open(options: CopyToJobModel) {
        if (this.jobs == null) this._getJobs();
        const result = super.open(options);

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getJobs() {
        const state: State = {
            filter: {
                filters: [
                    {
                        filters: [JobStatus.Pending, JobStatus.InProgress, JobStatus.Completed]
                            .map(e => ({ field: 'status', operator: 'eq', value: e })),
                        logic: 'or'
                    }
                ],
                logic: 'and'
            },
            sort: [
                { field: 'date', dir: 'asc' }
            ]
        };

        this._subscriptions.push(
            this._jobsService.read(state)
                .pipe(
                    tap(e => this.jobs = (e.data as IJobReadModel[]).map(e => new SelectableJob(e))),
                )
                .subscribe()
        );
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