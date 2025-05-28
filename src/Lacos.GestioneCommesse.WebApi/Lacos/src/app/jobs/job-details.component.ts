import { Component, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { ActivatedRoute, Params } from '@angular/router';
import { JobsService } from '../services/jobs/jobs.service';
import { filter, switchMap, tap } from 'rxjs';
import { Job } from '../services/jobs/models';
import { AddressModel } from '../shared/models/address.model';
import { AddressesService } from '../services/addresses.service';
import { JobModalComponent } from './job-modal.component';

@Component({
    selector: 'app-job-details',
    templateUrl: 'job-details.component.html'
})
export class JobDetailsComponent extends BaseComponent {

    @ViewChild('jobDetailModal', { static: true })
    jobModal: JobModalComponent;

    constructor(
        private readonly _service: JobsService,
        private readonly _addressService: AddressesService,
        private readonly _route: ActivatedRoute
    ) {
        super();
    }

    _jobId: number;
    job: Job;
    address: AddressModel;

    ngOnInit() {
        this._subscribeRouteParams();
    }

    private _subscribeRouteParams() {
        this._route.queryParams
            .pipe(
                tap(e => this._setParams(e))
            )
            .subscribe();
    }

    private _setParams(params: Params) {
        this._jobId = isNaN(+params['jobId']) ? null : +params['jobId'];
        this._read();
    }

    protected _read() {
        this._subscriptions.push(
            this._service.get(this._jobId)
                .pipe(
                    tap(e => {
                        this.job = e;
                        this._readAddress()
                    })
                )
                .subscribe()
        );
    }

    protected _readAddress() {
        this._subscriptions.push(
            this._addressService.getAddress(this.job.addressId)
                .pipe(
                    tap(e => this.address = e)
                )
                .subscribe()
        );
    }

    edit() {
        this._subscriptions.push(
            this._service.get(this._jobId)
                .pipe(
                    switchMap(e => this.jobModal.open(e)),
                    tap(e => !e && this._read()),
                    filter(e => e),
                    switchMap(() => this._service.update(this.jobModal.options)),
                    tap(() => this._read())
                )
                .subscribe()
        );
    }
}
