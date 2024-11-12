import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Activity, ActivityStatus, IActivityReadModel, activityStatusNames } from '../services/activities/models';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';
import { UserService } from '../services/security/user.service';
import { User } from '../services/security/models';
import { StorageService } from '../services/common/storage.service';
import { MessagesService } from '../services/messages/messages.service';
import { MessageModalOptions, MessageModel, MessagesListReadModel } from '../services/messages/models';
import { MessageModalComponent } from './message-modal.component';
import { State } from '@progress/kendo-data-query';
import { JobModalComponent } from '../jobs/job-modal.component';
import { ActivityModalComponent, ActivityModalOptions } from '../activities/activity-modal.component';
import { PurchaseOrderModalComponent, PurchaseOrderModalOptions } from '../purchase-order/purchase-order-modal.component';
import { TicketModalComponent } from '../ticket/ticket-modal.component';
import { JobsService } from '../services/jobs/jobs.service';
import { ActivitiesService } from '../services/activities/activities.service';
import { TicketsService } from '../services/tickets/tickets.service';
import { PurchaseOrdersService } from '../services/purchase-orders/purchase-orders.service';
import { ActivityTypesService } from '../services/activityTypes.service';
import { ActivityTypeModel } from '../shared/models/activity-type.model';

@Component({
    selector: 'app-interventionmessages-list',
    templateUrl: 'interventionmessages-list.component.html'
})
export class InterventionMessagesListComponent extends BaseComponent implements OnInit {

    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    @ViewChild('jobModal', { static: true }) jobModal: JobModalComponent;
    @ViewChild('activityModal', { static: true }) activityModal: ActivityModalComponent;
    @ViewChild('purchaseOrderModal', { static: true }) purchaseOrderModal: PurchaseOrderModalComponent;
    @ViewChild('ticketModal', { static: true }) ticketModal: TicketModalComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'date', dir: 'desc' }]
    };

    private cellArgs: CellClickEvent;
    user: User;
    currentOperator: OperatorModel;
    unreadCounter: number;
    hasFilter: boolean;
    targetOperatorsArray: number[];
    activityTypes: ActivityTypeModel[];
    selectedActivityTypeId: number;
    hasFilterUnread: boolean;
    screenWidth: number;

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _activityTypesService: ActivityTypesService,
        private readonly _service: MessagesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _user: UserService,
        private readonly _operatorsService: OperatorsService,
        private readonly _jobsService: JobsService,
        private readonly _activityService: ActivitiesService,
        private readonly _ticketsService: TicketsService,
        private readonly _purchaseOrdersService: PurchaseOrdersService,
        private readonly _storageService: StorageService
    ) {
        super();
    }

    ngOnInit() {
        this.hasFilterUnread = true;
        this._getActivityTypes();
        this._resumeState();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
        this.updateScreenSize();
      }
    
      @HostListener('window:resize', ['$event'])
      onResize(event: Event): void {
        this.updateScreenSize();
      }
    
      private updateScreenSize(): void {
        this.screenWidth = window.innerWidth -44;
        if (this.screenWidth > 1876) this.screenWidth = 1876;
        if (this.screenWidth < 1400) this.screenWidth = 1400;     
      }


    private _resumeState() {
        const savedState = this._storageService.get<State>(window.location.hash, true);
        if (savedState == null) return;
        this.gridState = savedState;
    }

    private _saveState() {
        this._storageService.save(this.gridState, window.location.hash, true);
        this.hasFilter = this.gridState.filter.filters.any();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._saveState();
        this._read();
    }

    onDblClick(): void {
        if (this.cellArgs.dataItem.ticketId) {
            this._subscriptions.push(
                this._ticketsService.get(this.cellArgs.dataItem.ticketId)
                    .pipe(
                        switchMap(e => this.ticketModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._ticketsService.update(this.ticketModal.options)),
                        tap(e => this._read())
                    )
                    .subscribe()
            );
            return;
        }
        if (this.cellArgs.dataItem.purchaseOrderId) {
            this._subscriptions.push(
                this._purchaseOrdersService.get(this.cellArgs.dataItem.purchaseOrderId)
                    .pipe(
                        map(e => new PurchaseOrderModalOptions(e)),
                        switchMap(e => this.purchaseOrderModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._purchaseOrdersService.update(this.purchaseOrderModal.options.purchaseOrder)),
                        tap(() => this._read())
                    )
                    .subscribe()
            );
            return;
        }
        if (this.cellArgs.dataItem.activityId) {
            this._subscriptions.push(
                this._activityService.get(this.cellArgs.dataItem.activityId)
                    .pipe(
                        map(e => new ActivityModalOptions(e)),
                        switchMap(e => this.activityModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._activityService.update(this.activityModal.options.activity)),
                        tap(() => this._read())
                    )
                    .subscribe()
            );
            return;
        }
        if (this.cellArgs.dataItem.jobId) {
            this._subscriptions.push(
                this._jobsService.get(this.cellArgs.dataItem.jobId)
                    .pipe(
                        switchMap(e => this.jobModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._jobsService.update(this.jobModal.options)),
                        tap(e => this._read())
                    )
                    .subscribe()
            );
            return;
        }
    }

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    protected _read() {
        this._subscriptions.push(
            this._service.getMessagesList(this.gridState, this.currentOperator.id)
                .pipe(
                    tap(e => {
                        this.data = e;
                        this._getUnreadCounter();
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
                    tap(() => {
                        this.filterUnread();
                    })
                )
                .subscribe()
        );
    }

    markAsRead(message: MessagesListReadModel) {
        this._subscriptions.push(
            this._service.markAsRead(message.id, this.currentOperator.id)
                .pipe(
                    tap(() => {
                        message.isRead = true;
                        //this.updateUnreadCounter();
                        this._messageBox.success('Commento letto');
                        this._read();
                    })
                )
                .subscribe()
        );
    }

    editMessage(message: MessagesListReadModel) {
        this._subscriptions.push(
            this._service.get(message.id)
                .pipe(
                    map(e => new MessageModalOptions(e, true)),
                    switchMap(e => this.messageModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.messageModal.options.message)),
                    //tap(() => this._afterMessageUpdated(this.messageModal.options.message))
                )
                .subscribe()
        );
    }

    replyMessage(message: MessagesListReadModel, replyAll: boolean) {
        this.targetOperatorsArray = [];
        this._subscriptions.push(
            this._service.getReplyTargetOperators(message.id, replyAll)
                .pipe(
                    tap(e => {
                        this.targetOperatorsArray = e;
                        this.createMessage(message, replyAll);
                    })
                )
                .subscribe()
        );
    }

    createMessage(message: MessagesListReadModel, replyAll: boolean) {
        const today = new Date();
        const newMessage = new MessageModel(message.id, today, null, this.currentOperator.id, message.jobId, message.activityId, message.ticketId, message.purchaseOrderId, false);
        const options = new MessageModalOptions(newMessage, true, replyAll, this.targetOperatorsArray);

        this._subscriptions.push(
            this.messageModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.createReply(newMessage, options.targetOperators.join(","))),
                    tap(() => this._read()),
                    tap(() => this._messageBox.success('Commento creato'))
                )
                .subscribe()
        );
    }

    readonly rowCallback = (context: RowClassArgs) => {
        if (!context.dataItem.isRead && context.dataItem.senderOperatorId != this.currentOperator.id) {
            return { unread: true };
        } else {
            return { read: true };
        }
        // const activity = context.dataItem as IActivityReadModel;

        // switch (true) {
        //     case activity.status === ActivityStatus.Completed:
        //         return { 'activity-completed': true };
        //     case activity.status === ActivityStatus.Pending:
        //         return { 'activity-pending': true };
        //     case activity.status === ActivityStatus.InProgress:
        //         return { 'activity-in-progress': true };
        //     case activity.status === ActivityStatus.Ready:
        //         return { 'activity-ready': true };
        //     case activity.status != ActivityStatus.Completed && !!activity.expirationDate && new Date(activity.expirationDate).addDays(1).isPast():
        //         return { 'activity-expired': true };
        //     default:
        //         return {};
        // }
    };

    resetFilter() {
        this.hasFilterUnread = false;
        this.selectedActivityTypeId = null;
        this.gridState.filter.filters = [];
        this._saveState();
        this._read();
    }

    filterUnread() {
        this.hasFilterUnread = true;
        this.gridState.filter.filters = [
            { field: 'senderOperatorId', operator: 'neq', value: this.currentOperator.id },
            { field: 'isRead', operator: 'eq', value: false },
            { field: 'isFromApp', operator: 'eq', value: true }
        ];
        this._saveState();
        this._read();
    }

    onActivityTypeChange() {
        if (this.hasFilterUnread)
            this.gridState.filter.filters = [
                { field: 'activityTypeId', operator: 'eq', value: this.selectedActivityTypeId },
                { field: 'isRead', operator: 'eq', value: false },
                { field: 'isFromApp', operator: 'eq', value: true }
            ];
        else
            this.gridState.filter.filters = [
                { field: 'activityTypeId', operator: 'eq', value: this.selectedActivityTypeId },
                { field: 'isFromApp', operator: 'eq', value: true }
            ];
        this._saveState();
        this._read();
    }

    _getUnreadCounter() {
        this._subscriptions.push(
            this._service.getUnreadCounter(this.currentOperator.id)
                .pipe(
                    tap(e => {
                        this.unreadCounter = e;
                    })
                )
                .subscribe()
        );
    }

    private _getActivityTypes() {
        this._subscriptions.push(
            this._activityTypesService.readActivityTypesList()
                .pipe(
                    tap(e => this.activityTypes = e)
                )
                .subscribe()
        );
    }
}
