import { Component, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Activity, ActivityStatus, IActivityReadModel, activityStatusNames } from '../services/activities/models';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';
import { UserService } from '../services/security/user.service';
import { User } from '../services/security/models';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { StorageService } from '../services/common/storage.service';
import { MessagesService } from '../services/messages/messages.service';
import { MessageModalOptions, MessageModel, MessagesListReadModel } from '../services/messages/models';
import { MessageModalComponent } from './message-modal.component';
import { State } from '@progress/kendo-data-query';
import { getToday } from '../services/common/functions';

@Component({
    selector: 'app-messages-list',
    templateUrl: 'messages-list.component.html'
})
export class MessagesListComponent extends BaseComponent implements OnInit {

    @ViewChild('messageModal', { static: true }) messageModal: MessageModalComponent;
    
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

    readonly activityStatusNames = activityStatusNames;

    constructor(
        private readonly _service: MessagesService,
        private readonly _messageBox: MessageBoxService,
        private readonly _user: UserService,
        private readonly _operatorsService: OperatorsService,
        private readonly _storageService: StorageService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
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
        if (!this.cellArgs.isEdited && this.cellArgs.dataItem.senderOperatorId == this.currentOperator.id) {
            this._subscriptions.push(
                this._service.get(this.cellArgs.dataItem.id)
                    .pipe(
                        map(e => new MessageModalOptions(e)),
                        switchMap(e => this.messageModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._service.update(this.messageModal.options.message)),
                        //tap(() => this._afterMessageUpdated(this.messageModal.options.message))
                    )
                    .subscribe()
            );
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
                    tap(() => this._read())
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
                    map(e => new MessageModalOptions(e)),
                    switchMap(e => this.messageModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.messageModal.options.message)),
                    //tap(() => this._afterMessageUpdated(this.messageModal.options.message))
                )
                .subscribe()
        );
    }

    createMessage(message: MessagesListReadModel, replyAll: boolean) {
        const today = new Date();
        const newMessage = new MessageModel(message.id, today, null, this.currentOperator.id, message.jobId, message.activityId, message.ticketId, message.purchaseOrderId);
        const options = new MessageModalOptions(newMessage);

        this._subscriptions.push(
            this.messageModal.open(options)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.createReply(newMessage,replyAll)),
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
        this.gridState.filter.filters = [];
        this._saveState();
        this._read();
    }

    filterUnread() {
        this.gridState.filter.filters = [
            { field: 'senderOperatorId', operator: 'neq', value: this.currentOperator.id }, 
            { field: 'isRead', operator: 'eq', value: false }
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

}
