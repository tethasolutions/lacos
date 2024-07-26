import { Component } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { SecurityService } from '../services/security/security.service';
import { Role, User } from '../services/security/models';
import { tap } from 'rxjs';
import { ActivityCounter, NewActivityCounter } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { TicketCounter } from '../services/tickets/models';
import { TicketsService } from '../services/tickets/tickets.service';
import { MessagesService } from '../services/messages/messages.service';
import { UserService } from '../services/security/user.service';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';

@Component({
    selector: 'lacos-home',
    templateUrl: 'home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent extends BaseComponent {

    activitiesCounters: ActivityCounter[];
    ticketsCounters: TicketCounter;
    newActivitiesCounter = new NewActivityCounter(0);
    unreadCounter: number;
    user: User;
    currentOperator: OperatorModel;

    readonly isAdmin: boolean;
    readonly isOperator: boolean;

    constructor(
        security: SecurityService,
        private readonly _activityService: ActivitiesService,
        private readonly _messagesService: MessagesService,
        private readonly _ticketService: TicketsService,
        private readonly _operatorsService: OperatorsService,
        private readonly _user: UserService
    ) {
        super();

        this.isAdmin = security.isAuthorized(Role.Administrator);
        this.isOperator = security.isAuthorized(Role.Operator);
    }

    ngOnInit() {
        this.ticketsCounters = new TicketCounter(0,0);
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
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

    protected _getCurrentOperator(userId: number) {
        this._subscriptions.push(
            this._operatorsService.getOperatorByUserId(userId)
                .pipe(
                    tap(e => this.currentOperator = e),
                    tap(() => this._getUnreadCounter())
                )
                .subscribe()
        );
    }

    _getUnreadCounter() {
        this._subscriptions.push(
            this._messagesService.getUnreadCounter(this.currentOperator.id)
                .pipe(
                    tap(e => {
                        this.unreadCounter = e;
                    })
                )
                .subscribe()
        );
    }
}
