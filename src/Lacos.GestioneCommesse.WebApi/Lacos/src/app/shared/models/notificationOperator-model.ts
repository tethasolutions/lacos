export class NotificationOperator {

    constructor(
        public id: number,
        public operatorId: number
    ) {}
    
    static build(o: NotificationOperator) {
        return new NotificationOperator(o.id, o.operatorId);
    }
}

export interface NotificationOperatorReadModel {
    readonly id: number;
    readonly operatorId: number;
    readonly operatorName: string;
}
