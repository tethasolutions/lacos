import { Dictionary } from '../common/models';

export enum ActivityStatus {

    Pending,
    InProgress,
    ReadyForCompletion,
    Completed

}

export const activityStatusNames: Dictionary<ActivityStatus, string> = {

    [ActivityStatus.Pending]: 'In attesa',
    [ActivityStatus.InProgress]: 'In corso',
    [ActivityStatus.ReadyForCompletion]: 'Pronta da evadere',
    [ActivityStatus.Completed]: 'Evasa'

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
    readonly jobHasHighPriority: boolean;
    readonly customer: string;
    readonly expirationDate: Date | string;

}

export class Activity {

    expirationDate: Date;

    constructor(
        readonly id: number,
        public status: ActivityStatus,
        readonly number: number,
        public description: string,
        public jobId: number,
        public customerAddressId: number,
        public typeId: number,
        expirationDate: Date | string
    ) {
        this.expirationDate = expirationDate ? new Date(expirationDate) : null;
    }

    toJSON() {
        const result = {
            ...this
        };

        result.expirationDate = result.expirationDate
            ? result.expirationDate.toOffsetString() as any
            : null;

        return result;
    }

    static build(o: Activity) {
        return new Activity(o.id, o.status, o.number, o.description, o.jobId, o.customerAddressId, o.typeId, o.expirationDate);
    }

}

export class ActivityDetail {

    readonly expirationDate: Date | string;

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
        readonly source: string,
        expirationDate: Date | string
    ) {
        this.expirationDate = expirationDate ? new Date(expirationDate) : null;
    }

    asActivity() {
        return new Activity(this.id, this.status, this.number, this.description, this.jobId,
            this.customerAddressId, this.typeId, this.expirationDate);
    }

    static build(o: ActivityDetail) {
        return new ActivityDetail(o.id, o.status, o.number, o.description, o.jobId, o.job, o.customerId,
            o.customer, o.customerAddressId, o.customerAddress, o.typeId, o.type, o.source, o.expirationDate);
    }

}

export class ActivityCounter {
    readonly id: number;
    readonly colorHex: string;
    readonly name: string;
    readonly active: number;
    readonly expired: number;
    
    constructor() {
        this.id = null;
        this.colorHex = null;
        this.name = null;
        this.active = null;
        this.expired = null;
    }

}