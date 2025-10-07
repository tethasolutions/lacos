
export class JobAccountingModel {

    constructor(
        public id: number,
        public jobId: number,
        public accountingTypeId: number,
        public amount: number,
        public note: string,
        public isPaid: boolean,
        public targetOperators: number[]
    ) {
    }

    static build(o: JobAccountingModel) {
        return new JobAccountingModel(o.id, o.jobId, o.accountingTypeId, o.amount, o.note, o.isPaid, o.targetOperators);
    }
    
}

export interface IJobAccountingReadModel {

    readonly id: number;
        readonly jobId: number;
        readonly jobCode: string;
        readonly jobReference: string;
        readonly customer: string;
        readonly accountingTypeId: number;
        readonly accountingTypeName: string;
        readonly accountingTypeOrder: number;
        readonly accountingTypeIsNegative: boolean;
        readonly generateAlert: boolean;
        readonly amount: number;
        readonly note: string;
        readonly isPaid: boolean;

}
