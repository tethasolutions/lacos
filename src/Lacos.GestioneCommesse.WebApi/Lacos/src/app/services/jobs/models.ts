import { Dictionary } from '../common/models';
import { JobAttachmentModel } from './job-attachment.model';

export enum JobStatus {

    Pending,
    InProgress,
    Completed,
    Billing,
    Billed

}

export const jobStatusNames: Dictionary<JobStatus, string> = {

    [JobStatus.Pending]: 'Attiva',
    [JobStatus.InProgress]: 'In corso',
    [JobStatus.Completed]: 'Completata',
    [JobStatus.Billing]: 'Da Fatturare',
    [JobStatus.Billed]: 'Fatturata'

};

export interface IJobReadModel {

    readonly id: number;
    readonly code: string;
    readonly date: Date | string;
    readonly description: string;
    readonly reference: string;
    readonly hasHighPriority: boolean;
    readonly status: JobStatus;
    readonly customerId: number;
    readonly customer: string;
    readonly addressId: number;
    readonly address: string;
    readonly canBeRemoved: boolean;
    readonly hasActivities: boolean;
    readonly hasAttachments: boolean;
    readonly hasPurchseOrders: boolean;

}

export class Job {

    date: Date;

    get code() {
        return `${this.number.toString().padStart(3, '0')}/${this.year}`;
    }

    constructor(
        readonly id: number,
        readonly number: number,
        public year: number,
        date: Date | string,
        public description: string,
        public reference: string,
        public hasHighPriority: boolean,
        readonly status: JobStatus,
        public customerId: number,
        public addressId: number,
        public attachments: JobAttachmentModel[]
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: Job) {
        const attachments = o.attachments.map(e => JobAttachmentModel.build(e));
        return new Job(o.id, o.number, o.year, o.date, o.description, o.reference, o.hasHighPriority, o.status, o.customerId, o.addressId, attachments);
    }

}

export class JobCopy {

    date: Date;

    constructor(
        readonly originalId: number,
        date: Date | string,
        public description: string,
        public reference: string,
        public customerId: number,
        public addressId: number
    ) {
        this.date = date ? new Date(date) : null;
    }
    
    static build(o: JobCopy) {
        return new JobCopy(o.originalId, o.date, o.description, o.reference, o.customerId, o.addressId);
    }

}
