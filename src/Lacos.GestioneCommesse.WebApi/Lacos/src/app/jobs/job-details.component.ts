import { Component } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { ActivatedRoute, Params } from '@angular/router';
import { JobsService } from '../services/jobs/jobs.service';
import { tap } from 'rxjs';
import { Job } from '../services/jobs/models';

@Component({
    selector: 'app-job-details',
    templateUrl: 'job-details.component.html'
})
export class JobDetailsComponent extends BaseComponent {

    constructor(
        private readonly _service: JobsService,
        private readonly _route: ActivatedRoute
    ) {
        super();
    }

    _jobId: number;
    job: Job;

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
                    tap(e => this.job = e)
                )
                .subscribe()
        );
    }
}
