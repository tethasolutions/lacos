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
        public isRead: boolean,
        public messageNotifications: MessageNotification[],
        public isFromApp: boolean,
        public targetOperators: string

    ){
        this.date = date ? new Date(date) : null;
    }

    static build(o: MessageReadModel, operatorId: number) {
        const messageNotifications = o.messageNotifications.map(e => MessageNotification.build(e));
        const isRead = messageNotifications.isEmpty() || messageNotifications.some(e => e.operatorId == operatorId && e.isRead) || messageNotifications.every(e => e.operatorId != operatorId); 
        return new MessageReadModel(o.id, o.date, o.note, o.operatorId, o.operatorName, o.jobId, o.activityId, o.ticketId, o.purchaseOrderId, o.elementCode, isRead, o.messageNotifications, o.isFromApp, o.targetOperators);
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
        public isFromApp: boolean
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: MessageModel) {
        return new MessageModel(o.id, o.date, o.note, o.operatorId, o.jobId, o.activityId, o.ticketId, o.purchaseOrderId, o.isFromApp);
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
    static build(o: MessageModel) {
        return new MessageModel(o.id, o.date, o.note, o.operatorId, o.jobId, o.activityId, o.ticketId, o.purchaseOrderId, o.isFromApp);
    }

}

export class MessageNotification {
    constructor(
        readonly messageId: number,
        readonly operatorId: number,
        readonly isRead: boolean
    ){}
    
    static build(o: MessageNotification) {
        return new MessageNotification(o.messageId, o.operatorId, o.isRead);
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
        public isFromApp: boolean,

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
