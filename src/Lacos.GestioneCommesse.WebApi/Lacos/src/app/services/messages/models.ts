import { NumberSymbol } from '@angular/common';
import { Dictionary } from '../common/models';

export class MessageReadModel {

    date: Date;

    constructor(
        readonly id: number,
        date: Date | string,
        public note: string,
        public operatorId: number,
        public operatorName: string,
        public jobId: number,
        public activityId: number,
        public ticketId: number,
        public purchaseOrderId: number,
        public elementCode: string,
        public isRead: boolean

    ){
        this.date = date ? new Date(date) : null;
    }

    static build(o: MessageReadModel) {
        return new MessageReadModel(o.id, o.date, o.note, o.operatorId, o.operatorName, o.jobId, o.activityId, o.ticketId, o.purchaseOrderId, o.elementCode, o.isRead);
    }
}

export class MessageModel {

    date: Date;

    constructor(
        readonly id: number,
        date: Date | string,
        public note: string,
        public operatorId: number,
        public jobId: number,
        public activityId: number,
        public ticketId: number,
        public purchaseOrderId: number,
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: MessageModel) {
        return new MessageModel(o.id, o.date, o.note, o.operatorId, o.jobId, o.activityId, o.ticketId, o.purchaseOrderId);
    }

}

