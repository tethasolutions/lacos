import { Dictionary } from '../common/models';

export enum JobStatus {

    Pending,
    InProgress,
    Completed

}

export const jobStatusNames: Dictionary<JobStatus, string> = {

    [JobStatus.Pending]: 'In attesa',
    [JobStatus.InProgress]: 'In corso',
    [JobStatus.Completed]: 'Completata'

};

export interface IJobReadModel {

    readonly id: number;
    readonly code: string;
    readonly date: Date | string;
    readonly description: string;
    readonly status: JobStatus;
    readonly customerId: number,
    readonly customer: string;
    readonly canBeRemoved: boolean;

}

export class Job {

    date: Date;

    get code() {
        return `${this.year}/${this.number}`;
    }

    constructor(
        readonly id: number,
        readonly number: number,
        public year: number,
        date: Date | string,
        public description: string,
        readonly status: JobStatus,
        public customerId: number
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: Job) {
        return new Job(o.id, o.number, o.year, o.date, o.description, o.status, o.customerId);
    }

}
