import { ActivityStatusEnum } from '../enums/activity-status.enum';

export class ActivityCalendarModel {
    id: number;
    description: string;
    start: Date;
    end: Date;
    status: ActivityStatusEnum;
    operatorId: number;
    customer: string;
    job: string;

    constructor() {
        this.id = null;
        this.description = null;
        this.start = new Date();
        this.end = new Date((new Date()).getTime() + (1000 * 60 * 60 * 24 * 2));
        this.status = ActivityStatusEnum.Planned;
        this.operatorId = null;
        this.job = null;
        this.customer = null;
    }
}
