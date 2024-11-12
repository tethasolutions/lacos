import { Dictionary } from '../common/models';
import { MessageReadModel } from '../messages/models';
import { JobAttachmentModel } from './job-attachment.model';

export enum JobStatus {

    Pending,
    InProgress,
    Completed,
    Billing,
    Billed,
    Suspended

}

export const jobStatusNames: Dictionary<JobStatus, string> = {

    [JobStatus.Pending]: 'ATTIVA',
    [JobStatus.InProgress]: 'IN CORSO',
    [JobStatus.Completed]: 'COMPLETATA',
    [JobStatus.Billing]: 'DA FATTURARE',
    [JobStatus.Billed]: 'FATTURATA',
    [JobStatus.Suspended]: 'SOSPESA'

};

export interface IJobReadModel {

    readonly id: number;
    readonly code: string;
    readonly date: Date | string;
    readonly expirationDate: Date | string;
    readonly description: string;
    readonly reference: string;
    readonly hasHighPriority: boolean;
    readonly status: JobStatus;
    readonly customerId: number;
    readonly customer: string;
    readonly customerContacts: string;
    readonly addressId: number;
    readonly address: string;
    readonly canBeRemoved: boolean;
    readonly hasActivities: boolean;
    readonly hasAttachments: boolean;
    readonly hasPurchseOrders: boolean;
    readonly hasInterventions: boolean;
    readonly referentId: number;
    readonly referentName: string;
    readonly unreadMessages: number;

}

export interface IJobProgressStatus {

    readonly jobId: number;
    readonly jobCode: string;
    readonly jobYear: number;
    readonly jobDate: Date | string;
    readonly jobReference: string;
    readonly jobStatus: JobStatus;
    readonly customerName: string;
    readonly activities_list: number;
    readonly activities_completed: number;
    readonly activities_progress: number;
    readonly interventions_list: number;
    readonly interventions_completed: number;
    readonly interventions_progress: number;
    readonly purchaseOrders_list: number;
    readonly purchaseOrders_completed: number;
    readonly purchaseOrders_progress: number;

}

export class Job {

    date: Date;
    expirationDate: Date;

    get code() {
        return `${this.number.toString().padStart(3, '0')}/${this.year}`;
    }

    constructor(
        readonly id: number,
        readonly number: number,
        public year: number,
        date: Date | string,
        expirationDate: Date | string,
        public description: string,
        public reference: string,
        public hasHighPriority: boolean,
        readonly status: JobStatus,
        public customerId: number,
        public addressId: number,
        public referentId: number,
        public attachments: JobAttachmentModel[],
        public messages: MessageReadModel[]
    ) {
        this.date = date ? new Date(date) : null;        
        this.expirationDate = expirationDate ? new Date(expirationDate) : null;
    }

    static build(o: Job) {
        const attachments = o.attachments.map(e => JobAttachmentModel.build(e));
        const messages = o.messages.map(e => MessageReadModel.build(e));
        return new Job(o.id, o.number, o.year, o.date, o.expirationDate, o.description, o.reference, o.hasHighPriority, o.status, o.customerId, o.addressId, o.referentId, attachments, messages);
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
        public addressId: number,
        public referentId: number
    ) {
        this.date = date ? new Date(date) : null;
    }
    
    static build(o: JobCopy) {
        return new JobCopy(o.originalId, o.date, o.description, o.reference, o.customerId, o.addressId, o.referentId);
    }

}
