import { Component } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';
import { tap } from 'rxjs';
import { ActivityCounter, NewActivityCounter } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { TicketCounter } from '../services/tickets/models';
import { TicketsService } from '../services/tickets/tickets.service';

@Component({
    selector: 'lacos-home',
    templateUrl: 'home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent extends BaseComponent {

    activitiesCounters: ActivityCounter[];
    ticketsCounters: TicketCounter;
    newActivitiesCounter = new NewActivityCounter(0);

    readonly isAdmin: boolean;
    readonly isOperator: boolean;

    constructor(
        security: SecurityService,
        private readonly _activityService: ActivitiesService,
        private readonly _ticketService: TicketsService
    ) {
        super();

        this.isAdmin = security.isAuthorized(Role.Administrator);
        this.isOperator = security.isAuthorized(Role.Operator);
    }

    ngOnInit() {
        this.ticketsCounters = new TicketCounter(0,0);
        this._getActivityTypes();
        this._getTicketsCounters();
        this._getNewActivitiesCounter();
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

    private _getTicketsCounters() {
        this._subscriptions.push(
            this._ticketService.readTicketsCounters()
                .pipe(
                    tap(e => {
                        this.ticketsCounters = e;
                    })
                )
                .subscribe()
        );
    }
    
    private _getNewActivitiesCounter() {
        this._subscriptions.push(
            this._activityService.readNewActivitiesCounter()
                .pipe(
                    tap(e => {
                        this.newActivitiesCounter = e;
                    })
                )
                .subscribe()
        );
    }
}
