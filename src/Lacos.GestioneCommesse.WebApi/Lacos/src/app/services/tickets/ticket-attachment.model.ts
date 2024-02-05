
export class TicketAttachmentModel {

    constructor(
        public id: number,
        public description: string,
        public fileName: string,
        public ticketId: number
    ) {
    }

    static build(o: TicketAttachmentModel) {
        return new TicketAttachmentModel(o.id, o.description, o.fileName, o.ticketId);
    }
}
