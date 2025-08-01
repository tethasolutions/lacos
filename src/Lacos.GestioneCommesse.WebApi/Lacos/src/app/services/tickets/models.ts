import { Dictionary } from '../common/models';
import { MessageReadModel } from '../messages/models';
import { TicketAttachmentModel } from './ticket-attachment.model';

export enum TicketStatus {
    Opened,
    InProgress,
    Resolved,
    Closed
}

export const ticketStatusNames: Dictionary<TicketStatus, string> = {

    [TicketStatus.Opened]: 'APERTO',
    [TicketStatus.InProgress]: 'IN CORSO',
    [TicketStatus.Resolved]: 'EVASO',
    [TicketStatus.Closed]: 'CHIUSO'

};

export interface ITicketReadModel {

    readonly id: number;
    readonly code: string;
    readonly date: Date | string;
    readonly description: string;
    readonly status: TicketStatus;
    readonly jobId: number;
    readonly addressId: number;
    readonly activityId: number;
    readonly purchaseOrderId: number;
    readonly customerId: number;
    readonly customerName: string;
    readonly customerEmail: string;
    readonly operatorName: string;
    //readonly canBeRemoved: boolean;
    readonly unreadMessages: number;
    readonly hasInterventions: boolean;

}

export class Ticket {

    date: Date;

    get code() {
        return `${this.year}/${this.number}`;
    }

    constructor(
        readonly id: number,
        readonly number: number,
        public year: number,
        date: Date | string,
        public description: string,
        public status: TicketStatus,
        public customerId: number,
        public addressId: number,
        public jobId: number,
        public activityId: number,
        public purchaseOrderId: number,
        public operatorId: number,
        public pictures: TicketAttachmentModel[],
        public messages: MessageReadModel[]
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: Ticket, operatorId: number) {
        const messages = o.messages.map(e => MessageReadModel.build(e,operatorId));
        return new Ticket(o.id, o.number, o.year, o.date, o.description, o.status, o.customerId, o.addressId, o.jobId, o.activityId, o.purchaseOrderId, o.operatorId, o.pictures, messages);
    }

}

export class TicketCounter {    
    constructor(
        readonly openedTickets: number,
        readonly newTickets: number
    ){}

    static build(o: TicketCounter) {
        return new TicketCounter(o.openedTickets, o.newTickets);
    }
}