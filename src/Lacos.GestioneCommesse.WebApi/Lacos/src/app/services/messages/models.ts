import { NumberSymbol } from '@angular/common';
import { Dictionary } from '../common/models';
import { OperatorModel } from 'src/app/shared/models/operator.model';

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

export class MessageModalOptions {
    constructor(
        readonly message: MessageModel,
        readonly isNewMessage: boolean,
        readonly replyAll: boolean = false,
        readonly targetOperators: number[] = []
    ) {
    }

}

export class MessagesListReadModel {

    date: Date;

    constructor(
        readonly id: number,
        date: Date | string,
        public note: string,
        public senderOperatorId: number,
        public senderOperator: string,
        public targetOperators: string,
        public isRead: boolean,

        public jobCode: string,
        public jobReference: string,
        public customerName: string,
        public activityTypeId: number,
        public activityType: string,
        public activityColor: string,
        public activityDescription: string,

        public jobId: number,
        public activityId: number,
        public ticketId: number,
        public purchaseOrderId: number,
        public elementType: string

    ){
        this.date = date ? new Date(date) : null;
    }

}
