import { Dictionary } from '../common/models';
import { MessageReadModel } from '../messages/models';
import { ActivityAttachmentModel } from './activity-attachment.model';

export enum ActivityStatus {

    Pending,
    InProgress,
    Ready,
    Completed

}

export const activityStatusNames: Dictionary<ActivityStatus, string> = {

    [ActivityStatus.Pending]: 'In attesa',
    [ActivityStatus.InProgress]: 'In corso',
    [ActivityStatus.Ready]: 'Pronto',
    [ActivityStatus.Completed]: 'Completata'

};

export interface IActivityReadModel {

    readonly id: number;
    readonly number: number;
    readonly jobId: number;
    readonly customerId: number;
    readonly shortDescription: string;
    readonly status: ActivityStatus;
    readonly address: string;
    readonly typeId: number;
    readonly type: string;
    readonly activityColor: string;
    readonly canBeRemoved: boolean;
    readonly jobCode: string;
    readonly jobReference: string;
    readonly jobHasHighPriority: boolean;
    readonly customer: string;
    readonly startDate: Date | string;
    readonly expirationDate: Date | string;
    readonly lastOperator: string;
    readonly referentId: number;
    readonly referentName: string;
    readonly isNewReferent: boolean;
    readonly isExpired: boolean;
    readonly isInternal: boolean;
    readonly statusLabel0: string;
    readonly statusLabel1: string;
    readonly statusLabel2: string;

}

export class Activity {

    startDate: Date;
    expirationDate: Date;

    constructor(
        readonly id: number,
        public status: ActivityStatus,
        readonly number: number,
        public shortDescription: string,
        public informations: string,
        public description: string,
        public jobId: number,
        public supplierId: number,
        public addressId: number,
        public typeId: number,
        public referentId: number,
        startDate: Date | string,
        expirationDate: Date | string,
        public statusLabel0: string,
        public statusLabel1: string,
        public statusLabel2: string,
        public attachments: ActivityAttachmentModel[],
        public messages: MessageReadModel[]
    ) {
        this.startDate = startDate ? new Date(startDate) : null;
        this.expirationDate = expirationDate ? new Date(expirationDate) : null;
    }

    toJSON() {
        const result = {
            ...this
        };

        result.startDate = result.startDate
            ? result.startDate.toOffsetString() as any
            : null;
        result.expirationDate = result.expirationDate
            ? result.expirationDate.toOffsetString() as any
            : null;

        return result;
    }

    static build(o: Activity) {
        const attachments = o.attachments.map(e => ActivityAttachmentModel.build(e));
        const messages = o.messages.map(e => MessageReadModel.build(e));

        return new Activity(o.id, o.status, o.number, o.shortDescription, o.informations, o.description, o.jobId, o.supplierId, o.addressId,
            o.typeId, o.referentId, o.startDate, o.expirationDate, o.statusLabel0, o.statusLabel1, o.statusLabel2, attachments, messages);
    }

}

export class ActivityDetail {

    readonly startDate: Date | string;
    readonly expirationDate: Date | string;

    constructor(
        readonly id: number,
        readonly status: ActivityStatus,
        readonly number: number,
        public shortDescription: string,
        public informations: string,
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
        startDate: Date | string,
        expirationDate: Date | string,
        readonly referentId: number,
        readonly referent: string,
        readonly statusLabel0: string,
        readonly statusLabel1: string,
        readonly statusLabel2: string,
        public attachments: ActivityAttachmentModel[],
        public messages: MessageReadModel[]
    ) {
        this.startDate = startDate ? new Date(startDate) : null;
        this.expirationDate = expirationDate ? new Date(expirationDate) : null;
    }

    asActivity() {
        return new Activity(this.id, this.status, this.number, this.shortDescription, this.informations, this.description, this.jobId, this.supplierId,
            this.addressId, this.typeId, this.referentId, this.startDate, this.expirationDate, this.statusLabel0, this.statusLabel1, this.statusLabel2, this.attachments, this.messages);
    }

    static build(o: ActivityDetail) {
        const attachments = o.attachments.map(e => ActivityAttachmentModel.build(e));
        const messages = o.messages.map(e => MessageReadModel.build(e));

        return new ActivityDetail(o.id, o.status, o.number, o.shortDescription, o.informations, o.description, o.jobId, o.job, o.customerId,
            o.customer, o.supplierId, o.addressId, o.address, o.typeId, o.type, o.startDate, o.expirationDate,
            o.referentId, o.referent, o.statusLabel0, o.statusLabel1, o.statusLabel2, attachments, messages);
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

export class NewActivityCounter {    
    constructor(
        readonly newActivities: number
    ){}

    static build(o: NewActivityCounter) {
        return new NewActivityCounter(o.newActivities);
    }
}