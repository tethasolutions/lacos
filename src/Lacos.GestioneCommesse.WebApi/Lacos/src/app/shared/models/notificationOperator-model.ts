export class NotificationOperator {
    id: number;
    operatorId: number;
    constructor() {
        this.id = null;
        this.operatorId = null;
    }
    
}

export interface NotificationOperatorReadModel {
    readonly id: number;
    readonly operatorId: number;
    readonly operatorName: string;
}
