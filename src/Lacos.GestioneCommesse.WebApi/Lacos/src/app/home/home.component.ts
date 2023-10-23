import { Component } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';
import { tap } from 'rxjs';
import { ActivityCounter } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';

@Component({
    selector: 'lacos-home',
    templateUrl: 'home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent extends BaseComponent {

    activitiesCounters: ActivityCounter[];

    readonly isAdmin: boolean;
    readonly isOperator: boolean;

    constructor(
        security: SecurityService,
        private readonly _activityService: ActivitiesService
    ) {
        super();

        this.isAdmin = security.isAuthorized(Role.Administrator);
        this.isOperator = security.isAuthorized(Role.Operator);
    }

    ngOnInit() {
        this._getActivityTypes();
    }

    private _getActivityTypes() {
        this._subscriptions.push(
            this._activityService.readActivityTypesCounters()
            .pipe(
                tap(e => {
                  this.activitiesCounters = e;
                })
            )
                .subscribe()
        );
    }
}
