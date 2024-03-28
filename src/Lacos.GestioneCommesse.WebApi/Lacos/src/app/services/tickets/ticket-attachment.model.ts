
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
    
    get isImage(): boolean {
        if (this.fileName.endsWith(".jpg") || this.fileName.endsWith(".png") || this.fileName.endsWith(".jpeg") || this.fileName.endsWith(".gif")) {
            return true;
        }
        else {
            return false;
        }
    }
}
