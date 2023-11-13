import { Component, OnInit, ViewChild } from '@angular/core';
import { GridComponent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { TicketsService } from '../services/tickets/tickets.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, switchMap, tap } from 'rxjs/operators';
import { TicketModalComponent } from './ticket-modal.component';
import { ITicketReadModel, Ticket, TicketStatus, ticketStatusNames } from '../services/tickets/models';
import { getToday } from '../services/common/functions';

@Component({
    selector: 'app-tickets',
    templateUrl: 'tickets.component.html'
})
export class TicketsComponent extends BaseComponent implements OnInit {

    @ViewChild('ticketModal', { static: true })
    ticketModal: TicketModalComponent;

    @ViewChild('grid', { static: true })
    grid: GridComponent;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
                {
                    filters: [TicketStatus.Opened, TicketStatus.InProgress]
                        .map(e => ({ field: 'status', operator: 'eq', value: e })),
                    logic: 'or'
                }
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'code', dir: 'desc' }]
    };
    expandedDetailKeys = new Array<number>();

    readonly ticketStatusNames = ticketStatusNames;

    constructor(
        private readonly _service: TicketsService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._read();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    create() {
        const today = getToday();
        const ticket = new Ticket(0,null,today.getFullYear(),today,null,TicketStatus.Opened,null);

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

}
