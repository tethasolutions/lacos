
export class JobAccountingModel {

    constructor(
        public id: number,
        public jobId: number,
        public accountingTypeId: number,
        public amount: number,
        public note: string,
        public isPaid: boolean
    ) {
    }

    static build(o: JobAccountingModel) {
        return new JobAccountingModel(o.id, o.jobId, o.accountingTypeId, o.amount, o.note, o.isPaid);
    }
    
}

export interface IJobAccountingReadModel {

    readonly id: number;
        readonly jobId: number;
        readonly jobCode: string;
        readonly jobReference: string;
        readonly accountingTypeId: number;
        readonly accountingTypeName: string;
        readonly generateAlert: boolean;
        readonly amount: number;
        readonly note: string;
        readonly isPaid: boolean;

}
