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
    readonly customerId: number;
    readonly description: string;
    readonly status: ActivityStatus;
    readonly address: string;
    readonly typeId: number;
    readonly type: string;
    readonly activityColor: string;
    readonly source: string;
    readonly canBeRemoved: boolean;
    readonly jobCode: string;
    readonly jobHasHighPriority: boolean;
    readonly customer: string;
    readonly expirationDate: Date | string;
    readonly lastOperator: string;

}

export class Activity {

    expirationDate: Date;

    constructor(
        readonly id: number,
        public status: ActivityStatus,
        readonly number: number,
        public description: string,
        public jobId: number,
        public supplierId: number,
        public addressId: number,
        public typeId: number,
        public attachmentDisplayName: string,
        public attachmentFileName: string,
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
        return new Activity(o.id, o.status, o.number, o.description, o.jobId, o.supplierId, o.addressId, o.typeId, o.attachmentDisplayName,
            o.attachmentFileName, o.expirationDate);
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
        readonly supplierId: number,
        readonly addressId: number,
        readonly address: string,
        readonly typeId: number,
        readonly type: string,
        readonly attachmentDisplayName: string,
        readonly attachmentFileName: string,
        readonly source: string,
        expirationDate: Date | string
    ) {
        this.expirationDate = expirationDate ? new Date(expirationDate) : null;
    }

    asActivity() {
        return new Activity(this.id, this.status, this.number, this.description, this.jobId, this.supplierId,
            this.addressId, this.typeId, this.attachmentDisplayName, this.attachmentFileName, this.expirationDate);
    }

    static build(o: ActivityDetail) {
        return new ActivityDetail(o.id, o.status, o.number, o.description, o.jobId, o.job, o.customerId,
            o.customer, o.supplierId, o.addressId, o.address, o.typeId, o.type, o.attachmentDisplayName, o.attachmentFileName, o.source, o.expirationDate);
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