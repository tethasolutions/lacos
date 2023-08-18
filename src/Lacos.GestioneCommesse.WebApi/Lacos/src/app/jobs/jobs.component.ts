import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { JobsService } from '../services/jobs.service';
import { AddressesService } from '../services/addresses.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { JobModalComponent } from '../job-modal/job-modal.component';
import { JobModel } from '../shared/models/job.model';
import { AddressModel } from '../shared/models/address.model';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { Router, NavigationEnd } from '@angular/router';
import { JobStatusEnum } from '../shared/enums/job-status.enum';
import { JobDetailModel } from '../shared/models/job-detail.model';

@Component({
    selector: 'app-jobs',
    templateUrl: './jobs.component.html',
    styleUrls: ['./jobs.component.scss']
})
export class JobsComponent extends BaseComponent implements OnInit {

    @ViewChild('jobModal', { static: true }) jobModal: JobModalComponent;
    //@ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    
    jobType: string;

    statusList: Array<string> = [];

    dataJobs: GridDataResult;
    stateGridJobs: State = {
        skip: 0,
        take: 10,
        filter: {
            filters: [],
            logic: 'and'
        },
        group: [],
        sort: [{ field: "jobDate", dir: "asc" }]
    };

    constructor(
        private readonly _jobsService: JobsService,
        private readonly _messageBox: MessageBoxService,
        private readonly _router: Router,
    ) {
        super();
    }

    ngOnInit() {
        this._readJobs();
    }

    dataStateChange(state: State) {
        this.stateGridJobs = state;
        this._readJobs();
    }

    protected _readJobs() {
        console.log(this.jobType);
        if (this.jobType == undefined) { return; }
        this._subscriptions.push(
            this._jobsService.readJobs(this.stateGridJobs, this.jobType)
                .pipe(
                    tap(e => {
                        console.log(e);
                        this.dataJobs = e;
                    })
                )
                .subscribe()
        );
    }

    createJob() {
        const request = new JobDetailModel();

        this._subscriptions.push(
            this.jobModal.open(request)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._jobsService.createJob(request)),
                    tap(e => {
                        this._messageBox.success(`Job ${request.description} creato`);
                    }),
                    tap(() => this._readJobs())
                )
                .subscribe()
        );
    }

    editJob(job: JobModel) {

        this._subscriptions.push(
            this._jobsService.getJobDetail(job.id)
                .pipe(
                    map(e => {
                        return e;
                    }),
                    switchMap(e => this.jobModal.open(e)),
                    filter(e => e),
                    map(() => this.jobModal.options),
                    switchMap(e => this._jobsService.updateJob(e, e.id)),
                    map(() => this.jobModal.options),
                    tap(e => this._messageBox.success(`Job '${e.description}' aggiornato`)),
                    tap(() => this._readJobs())
                )
                .subscribe()
        );
    }

    deleteJob(job: JobModel) {
        this._messageBox.confirm(`Sei sicuro di voler cancellare la richiesta ${job.code}?`, 'Conferma l\'azione').subscribe(result => {
            if (result == true) {
                this._messageBox.confirm(`Cancellando la richiesta ${job.code} verranno rimossi anche i relativi preventivi, ordini e interventi. Continuare?`, 'Conferma l\'azione').subscribe(result => {
                    if (result == true) {
                        this._subscriptions.push(
                            this._jobsService.deleteJob(job.id)
                                .pipe(
                                    tap(e => this._messageBox.success(`Richiesta ${job.code} cancellato con successo`)),
                                    tap(() => this._readJobs())
                                )
                                .subscribe()
                        );
                    }
                });
            }
        });
    }

}
