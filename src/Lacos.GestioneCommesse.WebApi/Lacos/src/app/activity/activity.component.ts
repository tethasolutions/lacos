import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseComponent } from '../shared/base.component';
import { filter, switchMap, tap } from 'rxjs';
import { Activity, ActivityDetail } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { NgForm } from '@angular/forms';
import { JobActivityModalComponent, JobActivityModalOptions } from '../jobs/job-activity-modal.component';

@Component({
    selector: 'app-activity',
    templateUrl: 'activity.component.html'
})
export class ActivityComponent extends BaseComponent implements OnInit {

    @ViewChild('form', { static: false })
    form: NgForm;

    @ViewChild('jobActivityModal', { static: true })
    jobActivityModal: JobActivityModalComponent;

    activity: ActivityDetail;

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _service: ActivitiesService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._subscribeRouteParams();
    }

    edit() {
        const activity = this.activity.asActivity();
        const options = new JobActivityModalOptions(this.activity.customerId, activity);

        this._subscriptions.push(
            this.jobActivityModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.update(activity)),
                    tap(() => this._afterActivityUpdated())
                )
                .subscribe()
        );
    }

    private _subscribeRouteParams() {
        this._subscriptions.push(
            this._route.data
                .pipe(
                    tap(e => this.activity = e['activity'] as ActivityDetail)
                )
                .subscribe()
        );
    }

    private _afterActivityUpdated() {
        this._messageBox.success(`Attività aggiornata.`);

        this._subscriptions.push(
            this._service.getDetail(this.activity.id)
                .pipe(
                    tap(e => this.activity = e)
                )
                .subscribe()
        );
    }

}
