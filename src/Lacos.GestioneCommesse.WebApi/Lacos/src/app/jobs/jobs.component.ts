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
import { ActivityModalComponent } from '../activity-modal/activity-modal.component';
import { ActivitiesService } from '../services/activities.service';
import { ActivityModel } from '../shared/models/activity.model';
import { ActivityStatusEnum } from '../shared/enums/activity-status.enum';

@Component({
    selector: 'app-jobs-active',
    templateUrl: './jobs-active.component.html',
    styleUrls: ['./jobs-active.component.scss']
})
export class JobsActiveComponent extends BaseComponent implements OnInit {

    @ViewChild('jobModal', { static: true }) jobModal: JobModalComponent;
    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    
    jobType: string;

    statusList: Array<string> = [];

    jobNotes: Array<NoteModel> = [];

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
        private readonly _activitiesService: ActivitiesService
    ) {
        super();
    }

    ngOnInit() {
        console.log(this._router.url);
        if (this._router.url === '/jobs/acceptance') { this.jobType = 'acceptance'; }
        if (this._router.url === '/jobs/active') { this.jobType = 'active'; }
        if (this._router.url === '/jobs/completed') { this.jobType = 'completed'; }
        if (this._router.url === '/jobs/billing') { this.jobType = 'billing'; }
        if (this._router.url === '/jobs/paid') { this.jobType = 'paid'; }
        this.statusList = Object.keys(ActivityStatusEnum);
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

    createActivity(job: JobModel) {
        const request = new ActivityModel();

        request.jobId = job.id;
        request.jobDescription = job.description;
        request.jobCode = job.code;
        request.customerName = job.customer.name;

        this.activityModal.loadData();

        this._subscriptions.push(
            this.activityModal.open(request)
                .pipe(
                    filter(e => e),
                    map(() => this.activityModal.options),
                    switchMap(e => this._activitiesService.createActivity(e)),
                    tap(e => this._messageBox.success(`Intervento creato`)),
                    tap(() => this._readJobs())
                )
                .subscribe()
        );
    }

    isVisibleNew(): boolean 
    {
        return this.jobType == "acceptance" ||
                this.jobType == "active";
    }  
   

}
