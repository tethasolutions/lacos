import { Dictionary } from '../common/models';

export enum TicketStatus {
    Opened,
    InProgress,
    Resolved,
    Canceled
}

export const ticketStatusNames: Dictionary<TicketStatus, string> = {

    [TicketStatus.Opened]: 'Aperto',
    [TicketStatus.InProgress]: 'In corso',
    [TicketStatus.Resolved]: 'Evaso',
    [TicketStatus.Canceled]: 'Annullato'

};

export interface ITicketReadModel {

    readonly id: number;
    readonly code: string;
    readonly date: Date | string;
    readonly description: string;
    readonly status: TicketStatus;
    readonly interventionId: number;
    readonly customerId: number;
    readonly customerName: string;
    readonly customerAddressId: number,
    readonly customerFullAddress: string;

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
        readonly status: TicketStatus,
        public customerId: number,        
        public customerAddressId: number
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: Ticket) {
        return new Ticket(o.id, o.number, o.year, o.date, o.description, o.status, o.customerId, o.customerAddressId);
    }

}
