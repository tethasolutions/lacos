import { Component, OnInit } from '@angular/core';
import { CopyActivityModel } from '../services/activities/models';
import { ModalFormComponent } from '../shared/modal.component';
import { tap } from 'rxjs';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobsService } from '../services/jobs/jobs.service';
import { State } from '@progress/kendo-data-query';
import { IJobReadModel } from '../services/jobs/models';

@Component({
    selector: 'app-copy-activity-modal',
    templateUrl: 'copy-activity-modal.component.html'
})
export class CopyActivityModalComponent extends ModalFormComponent<CopyActivityModel> implements OnInit {

    jobs: SelectableJob[];

    constructor(
        messageBox: MessageBoxService,
        private readonly _jobsService: JobsService,
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._getJobs();
    }

    override open(options: CopyActivityModel) {
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
                filters: [],
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