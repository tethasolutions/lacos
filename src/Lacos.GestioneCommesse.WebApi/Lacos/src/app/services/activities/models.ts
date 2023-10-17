import { Dictionary } from '../common/models';

export enum ActivityStatus {

    Pending,
    InProgress,
    Completed

}

export const activityStatusNames: Dictionary<ActivityStatus, string> = {

    [ActivityStatus.Pending]: 'In attesa',
    [ActivityStatus.InProgress]: 'In corso',
    [ActivityStatus.Completed]: 'Completata'

};

export interface IActivityReadModel {

    readonly id: number;
    readonly number: number;
    readonly jobId: number;
    readonly description: string;
    readonly status: ActivityStatus;
    readonly customerAddress: string;
    readonly type: string;
    readonly source: string;
    readonly canBeRemoved: boolean;
    readonly jobCode: string;
    readonly customer: string;

}

export class Activity {

    constructor(
        readonly id: number,
        readonly status: ActivityStatus,
        readonly number: number,
        public description: string,
        readonly jobId: number,
        public customerAddressId: number,
        public typeId: number
    ) {
    }

    static build(o: Activity) {
        return new Activity(o.id, o.status, o.number, o.description, o.jobId, o.customerAddressId, o.typeId);
    }

}

export class ActivityDetail {

    constructor(
        readonly id: number,
        readonly status: ActivityStatus,
        readonly number: number,
        readonly description: string,
        readonly jobId: number,
        readonly job: number,
        readonly customerId: number,
        readonly customer: string,
        readonly customerAddressId: number,
        readonly customerAddress: number,
        readonly typeId: number,
        readonly type: string,
        readonly source: string
    ) {
    }

    asActivity() {
        return new Activity(this.id, this.status, this.number, this.description, this.jobId,
            this.customerAddressId, this.typeId);
    }

    static build(o: ActivityDetail) {
        return new ActivityDetail(o.id, o.status, o.number, o.description, o.jobId, o.job, o.customerId,
            o.customer, o.customerAddressId, o.customerAddress, o.typeId, o.type, o.source);
    }

}
