import { Component, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridComponent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { TicketsService } from '../services/tickets/tickets.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { TicketModalComponent } from './ticket-modal.component';
import { ITicketReadModel, Ticket, TicketStatus, ticketStatusNames } from '../services/tickets/models';
import { getToday } from '../services/common/functions';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { StorageService } from '../services/common/storage.service';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { User } from '../services/security/models';
import { UserService } from '../services/security/user.service';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';

@Component({
    selector: 'app-tickets',
    templateUrl: 'tickets.component.html'
})
export class TicketsComponent extends BaseComponent implements OnInit {

    @ViewChild('ticketModal', { static: true })
    ticketModal: TicketModalComponent;

    @ViewChild('grid', { static: true })
    grid: GridComponent;
    
    @ViewChild('customerModal', { static: true }) customerModal: CustomerModalComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
                this._buildStatusFilter(),
                this._buildJobIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'date', dir: 'desc' }]
    };
    expandedDetailKeys = new Array<number>();

    readonly ticketStatusNames = ticketStatusNames;
    private cellArgs: CellClickEvent;
    user: User;
    currentOperator: OperatorModel;
    private _jobId: number;

    constructor(
        private readonly _service: TicketsService,
        private readonly _messageBox: MessageBoxService,
        private readonly _route: ActivatedRoute,
        private router: Router,
        private readonly _user: UserService,
        private readonly _operatorsService: OperatorsService,
        private readonly _customerService: CustomerService,
        private readonly _storageService: StorageService
    ) {
        super();
    }

    ngOnInit() {
        this._resumeState();
        this._subscribeRouteParams();
        this.user = this._user.getUser();
        this._getCurrentOperator(this.user.id);
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._saveState();
        this._read();
    }

    private _resumeState() {
        const savedState = this._storageService.get<State>(window.location.hash, true);
        if (savedState == null) return;
        this.gridState = savedState;
    }

    private _saveState() {
        this._storageService.save(this.gridState,window.location.hash,true);
    }
    
    create() {
        const today = getToday();
        const ticket = new Ticket(0,null,today.getFullYear(),today,null,TicketStatus.Opened,null,null,null,this.currentOperator.id,[],[]);

        this._subscriptions.push(
            this.ticketModal.open(ticket)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(ticket)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    edit(ticket: ITicketReadModel) {
        this._subscriptions.push(
            this._service.get(ticket.id)
                .pipe(
                    switchMap(e => this.ticketModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.ticketModal.options)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    onDblClick(): void {
        if (!this.cellArgs.isEdited) {
            this._subscriptions.push(
                this._service.get(this.cellArgs.dataItem.id)
                    .pipe(
                        switchMap(e => this.ticketModal.open(e)),
                        filter(e => e),
                        switchMap(() => this._service.update(this.ticketModal.options)),
                        tap(e => this._afterSaved(e))
                    )
                    .subscribe()
            );
        }
    }
    
    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    askRemove(ticket: ITicketReadModel) {
        const text = `Sei sicuro di voler rimuovere il ticket ${ticket.code}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(ticket.id)),
                    tap(() => this._afterRemoved(ticket))
                )
                .subscribe()
        );
    }
    
    expandDetailsBy = (ticket: ITicketReadModel) => {
        return ticket.id;
    };

    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    private _afterSaved(ticket: Ticket) {
        this._messageBox.success(`Ticket ${ticket.code} salvata.`);

        if (ticket.activityId != null) {
            this.router.navigate(['/activities?jobId=' + ticket.jobId]);
        }

        this._read();
    }

    private _afterRemoved(ticket: ITicketReadModel) {
        const text = `Ticket ${ticket.code} rimossa.`;

        this._messageBox.success(text);

        this._read();
    }
    
    readonly rowCallback = (context: RowClassArgs) => {
        const ticket = context.dataItem as ITicketReadModel;

        switch (true) {
            case ticket.status === TicketStatus.Canceled:
                return { 'ticket-canceled': true };
            case ticket.status === TicketStatus.InProgress:
                return { 'ticket-inprogress': true };
            case ticket.status === TicketStatus.Opened:
                return { 'ticket-opened': true };
            case ticket.status === TicketStatus.Resolved:
                return { 'ticket-resolved': true };
            default:
                return {};
        }
    };

    openCustomer(customerId: number): void {
        this._subscriptions.push(
            this._customerService.getCustomer(customerId)
                .pipe(
                    map(e => {
                        return Object.assign(new CustomerModel(), e);
                    }),
                    switchMap(e => this.customerModal.open(e)),
                    filter(e => e),
                    map(() => this.customerModal.options),
                    switchMap(e => this._customerService.updateCustomer(e, customerId)),
                    map(() => this.customerModal.options),
                    tap(e => this._messageBox.success(`Cliente ${e.name} aggiornato`)),
                    tap(() => this._read())
                )
                .subscribe()
        );
    }

    protected _getCurrentOperator(userId: number) {
        this._subscriptions.push(
            this._operatorsService.getOperatorByUserId(userId)
                .pipe(
                    tap(e => this.currentOperator = e)
                )
                .subscribe()
        );
    }

    private _buildJobIdFilter() {
        const that = this;
        
        return {
            field: 'jobId',
            get operator() {
                return that._jobId
                    ? 'eq'
                    : 'neq'
            },
            get value() {
                return that._jobId
                   ? that._jobId
                    : 0;
            }
        };
    }

    private _buildStatusFilter() {
        const that = this;

        return {
            get field() {
                return that._jobId
                    ? 'id'
                    : undefined
            },
            get operator() {
                return that._jobId
                    ? 'isnotnull'
                    : undefined
            },
            get filters() {
                return that._jobId
                    ? undefined
                    : [TicketStatus.Opened, TicketStatus.InProgress]
                        .map(e => ({ field: 'status', operator: 'eq', value: e }))
            },
            logic: 'or'
        };
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
}
