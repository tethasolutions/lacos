import { Dictionary } from '../common/models';

export enum JobStatus {

    Pending,
    InProgress,
    Completed,
    Billing

}

export const jobStatusNames: Dictionary<JobStatus, string> = {

    [JobStatus.Pending]: 'Attiva',
    [JobStatus.InProgress]: 'In corso',
    [JobStatus.Completed]: 'Completata',
    [JobStatus.Billing]: 'Da Fatturare'

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
        public addressId: number
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: Job) {
        return new Job(o.id, o.number, o.year, o.date, o.description, o.reference, o.hasHighPriority, o.status, o.customerId, o.addressId);
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
